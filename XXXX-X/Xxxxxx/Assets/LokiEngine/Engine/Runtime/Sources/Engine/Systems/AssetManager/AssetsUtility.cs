#if UNITY_ADDRESSABLE_SYSTEM
using System.Threading.Tasks;

#if UNIRX_LIB
using UniRx.Async;
#endif

namespace Loki
{
	public static class AssetsUtility
	{
		public static async Task<TObject> ToTask<TObject>(this AsyncOperationHandle<TObject> handle)
		{
#if UNITY_WEBGL
			TObject assetResult = default;
			bool assetLoaded = false;
			handle.Completed += ao =>
			{
				assetLoaded = true;
				var result = ao.Result;
				assetResult = result;
			};

			while (!assetLoaded)
			{
				await Task.Delay(1);
			}

#else
			await handle.Task;
			var assetResult = handle.Result;
#endif
			return assetResult;
		}

#if UNIRX_LIB
		public static UniTask<T> ToUniTask<T>(this AsyncOperationHandle<T> asyncOperation) =>
			new UniTask<T>(new AsyncOperationAwaiter<T>(asyncOperation));

		struct AsyncOperationAwaiter<T> : IAwaiter<T>
		{
			AsyncOperationHandle<T> _asyncOperation;
			Action<AsyncOperationHandle<T>> _continuationAction;
			AwaiterStatus _status;
			T _result;

			public AsyncOperationAwaiter(AsyncOperationHandle<T> asyncOperation)
			{
				_status = asyncOperation.IsDone ? AwaiterStatus.Succeeded : AwaiterStatus.Pending;
				_result = asyncOperation.IsDone ? asyncOperation.Result : default;
				_asyncOperation = _status.IsCompleted() ? default : asyncOperation;
				_continuationAction = null;
			}

			bool IAwaiter.IsCompleted => _status.IsCompleted();
			AwaiterStatus IAwaiter.Status => _status;
			void IAwaiter.GetResult() => ((IAwaiter<T>)this).GetResult();
			T IAwaiter<T>.GetResult()
			{
				if (_status == AwaiterStatus.Succeeded) return _result;

				if (_status == AwaiterStatus.Pending)
				{
					// first timing of call
					if (_asyncOperation.IsDone)
					{
						_status = AwaiterStatus.Succeeded;
					}
					else
					{
						throw new InvalidOperationException("Not yet completed.");
					}
				}

				if (_continuationAction != null && _asyncOperation.IsValid())
				{
					_asyncOperation.Completed -= _continuationAction;
					_continuationAction = null;
				}

				if (_asyncOperation.IsValid())
				{
					_result = _asyncOperation.Result;
					_asyncOperation = default; // remove reference.
				}

				return _result;
			}

			public void OnCompleted(Action continuation)
			{
				UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				if (_continuationAction != null)
					throw new InvalidOperationException("continuation is already registered.");
				_continuationAction = _ => continuation.Invoke();
				_asyncOperation.Completed += _continuationAction;
			}
		}
#endif
	}
}
#endif

