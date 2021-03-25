using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Loki
{
	public partial class AssetManager
	{
		class AsyncInstAssetInfo
		{
			internal enum EAsyncState
			{
				ToBeContinue,
				ToStop,
			}

			internal GameObject source;
			internal IList<GameObject> result;
			internal Action<GameObject, int> onInst;
			internal Action<GameObject> onCompleted;
			internal Predicate<int> toBeContinue;
			internal int totalCount;
			internal int completedCount;
			internal EAsyncState state;

			static AsyncInstAssetInfo()
			{
				MemoryPool<AsyncInstAssetInfo>.defaultInstance.SetNew(info =>
				{
					if (info == null)
					{
						return new AsyncInstAssetInfo();
					}
					info.Reset();
					return info;
				});

				MemoryPool<AsyncInstAssetInfo>.defaultInstance.SetReset(info =>
				{
					if (info != null)
					{
						info.Reset();
					}
				});
			}

			internal static AsyncInstAssetInfo Alloc(GameObject source, int count, Action<GameObject, int> onInst, Action<GameObject> onCompleted, IList<GameObject> result = null, Predicate<int> toBeContinue = null)
			{
				return MemoryPool<AsyncInstAssetInfo>.defaultInstance.Pop(info =>
				{
					if (info == null)
					{
						return new AsyncInstAssetInfo();
					}
					info.source = source;
					info.result = result;
					info.toBeContinue = toBeContinue;
					info.onInst = onInst;
					info.onCompleted = onCompleted;
					info.totalCount = count;
					info.completedCount = 0;
					info.state = EAsyncState.ToBeContinue;
					return info;
				});
			}

			internal static void Dealloc(ref AsyncInstAssetInfo info)
			{
				MemoryPool<AsyncInstAssetInfo>.defaultInstance.Push(info);
				info = null;
			}

			public override string ToString()
			{
				if (source == null)
				{
					return "Empty task";
				}
				return string.Concat("Instantiate ", source.name);
			}

			internal void Instantiate()
			{
				int idx = completedCount;
				if ((source != null) && (toBeContinue == null || toBeContinue(idx)))
				{
					var o = UnityObject.Instantiate(source);
					o.name = source.name;
					completedCount += 1;
					if (result != null)
						result.Add(o);
					Misc.SafeInvoke(onInst, o, idx);
					if (completedCount >= totalCount)
					{
						state = AsyncInstAssetInfo.EAsyncState.ToStop;
						Misc.SafeInvoke(onInst, o, idx);
					}
				}
				else
				{
					state = AsyncInstAssetInfo.EAsyncState.ToStop;
				}
			}

			internal void Reset()
			{
				source = null;
				result = null;
				toBeContinue = null;
				onInst = null;
				onCompleted = null;
				totalCount = 0;
				completedCount = 0;
				state = EAsyncState.ToStop;
			}
		}

		/// <summary>
		/// The assets info
		/// </summary>
		private readonly List<AsyncInstAssetInfo> mAsyncInstAssetsInfo = new List<AsyncInstAssetInfo>();
		/// <summary>
		/// Async inst transiting
		/// </summary>
		private bool mAsyncInstTransiting = false;
		private readonly Stopwatch mAsyncInstWatch = new Stopwatch();

		/// <summary>
		/// the instantiate time 1 sec / 30 fps 
		/// </summary>
		private float mAsyncInstPfWarning = 1000 / 30;

		/// <summary>
		/// async to instantiate the gameObjects.
		/// Ignore the real-time FPS.
		/// </summary>
		/// <param name="source">which one</param>
		/// <param name="count">the amount</param>
		/// <param name="onInst">callback when instaniate every one</param>
		/// <param name="result">the container if you want to collect the result</param>
		/// <param name="autoCollectResult">true if you want to collect the result, but you dont need the cache result.</param>
		/// <param name="toBeContinue">To be continue or stop immediately. true if continue, false otherwise. it accept a index of cloning gameObject</param>
		/// <param name="millisecondsDelay">The interval (:ms) between instantiations</param>
		/// <returns>the Task handler</returns>
		public static async Task<IList<GameObject>> InstantiateAsync(
			GameObject source, int count, Action<GameObject, int> onInst,
			IList<GameObject> result = null,
			bool autoCollectResult = true,
			Predicate<int> toBeContinue = null,
			int millisecondsDelay = 10)
		{
			if ((result == null) && autoCollectResult)
			{
				result = new List<GameObject>(count);
			}
			for (int i = 0; i < count; i++)
			{
				if (toBeContinue == null || toBeContinue(i))
				{
					if (millisecondsDelay > 0)
					{
						await Task.Delay(millisecondsDelay);
					}
					var clone = UnityObject.Instantiate(source);
					clone.name = source.name;
					Misc.SafeInvoke(onInst, clone, i);
					if (clone != null && result != null)
					{
						result.Add(clone);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// async to instantiate the gameObjects.
		/// if the FPS is lower. the interval between instantiations will expend.
		/// </summary>
		/// <param name="source">which one</param>
		/// <param name="count">the amount</param>
		/// <param name="onInst">callback when instaniate every one</param>
		/// <param name="result">the container if you want to collect the result</param>
		/// <param name="autoCollectResult">true if you want to collect the result, but you dont need the cache result.</param>
		/// <param name="toBeContinue">To be continue or stop immediately. true if continue, false otherwise. it accept a index of cloning gameObject</param>
		public void InstantiateAsync(GameObject source, int count, Action<GameObject, int> onInst, Action<GameObject> onCompleted, IList<GameObject> result = null, bool autoCollectResult = true, Predicate<int> toBeContinue = null)
		{
			if (count <= 0)
			{
				throw new Exception(string.Format("InstantiateAsync do not allow the count ({0}) equals to or less than zero.", count));
			}
			if (result == null && autoCollectResult)
			{
				result = new List<GameObject>(count);
			}
			mAsyncInstAssetsInfo.Add(AsyncInstAssetInfo.Alloc(source, count, onInst, onCompleted, result, toBeContinue));
		}

		/// <summary>
		/// Stop the InstantiateAsync() task
		/// </summary>
		/// <param name="source">which one</param>
		public int StopInstantiate(GameObject source, bool forceStopAll = false)
		{
			int stopTaskCount = 0;
			int length = mAsyncInstAssetsInfo.Count;
			for (int i = 0; i < length; i++)
			{
				var task = mAsyncInstAssetsInfo[i];
				if (task != null && task.source == source && task.state == AsyncInstAssetInfo.EAsyncState.ToBeContinue)
				{
					task.state = AsyncInstAssetInfo.EAsyncState.ToStop;
					if (!forceStopAll)
					{
						break;
					}
				}
			}
			CleanupInvalidTasks();
			return stopTaskCount;
		}

		private void CleanupInvalidTasks()
		{
			if (mAsyncInstTransiting)
			{
				DebugUtility.LogError(LoggerTags.AssetManager, "Check the error call.");
				return;
			}
			mAsyncInstTransiting = true;
			int length = mAsyncInstAssetsInfo.Count;
			for (int i = 0; i < length; i++)
			{
				var task = mAsyncInstAssetsInfo[i];
				if (task.state == AsyncInstAssetInfo.EAsyncState.ToStop)
				{
					task.Reset();
					AsyncInstAssetInfo.Dealloc(ref task);
				}
			}
			mAsyncInstTransiting = false;
			mAsyncInstAssetsInfo.RemoveAll(task => task.state == AsyncInstAssetInfo.EAsyncState.ToStop);
		}

		private void DoInstantiateTask(float delta)
		{
			if (mAsyncInstTransiting)
			{
				DebugUtility.LogError(LoggerTags.AssetManager, "Check the error call.");
				return;
			}
			mAsyncInstTransiting = true;
			mAsyncInstWatch.Restart();

			ProfilingUtility.BeginSample("DoInstantiateTask");
			int length = mAsyncInstAssetsInfo.Count;
			for (int i = 0; i < length; i++)
			{
				var task = mAsyncInstAssetsInfo[i];
				if (task != null && task.state == AsyncInstAssetInfo.EAsyncState.ToBeContinue)
				{
					ProfilingUtility.BeginSample(task.ToString());
					task.Instantiate();
					ProfilingUtility.EndSample();
				}
				// pf warning
				if (mAsyncInstWatch.ElapsedMilliseconds >= mAsyncInstPfWarning)
				{
					break;
				}
			}
			mAsyncInstWatch.Stop();
			ProfilingUtility.EndSample();
			mAsyncInstTransiting = false;
			CleanupInvalidTasks();
		}
	}
}
