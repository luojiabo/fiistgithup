using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Loki.UI
{
	public partial class WindowManager : USingletonObject<WindowManager>, ISystem
	{
		/// <summary>
		/// Windows渲染的SortingOrder间距
		/// </summary>
		private static readonly int msDeltaOrder = 10;
		/// <summary>
		/// 等待加载队列
		/// </summary>
		private static readonly Queue<IEnumerator> msWaitingLoadQueue = new Queue<IEnumerator>();
		/// <summary>
		/// 未使用的Windows
		/// </summary>
		private static readonly List<Window> msUnusedWindows = new List<Window>();
		/// <summary>
		/// 正使用的Windows
		/// </summary>
		private static readonly List<Window> msUsedWindows = new List<Window>();
		/// <summary>
		/// 遮罩Order列表
		/// </summary>
		private static readonly List<int> msMaskOrderMapper = new List<int>();

		/// <summary>
		/// 遮罩Order
		/// </summary>
		public static int maskOrder
		{
			get
			{
				if (msMaskOrderMapper.Count <= 0) return 0;
				return msMaskOrderMapper[msMaskOrderMapper.Count - 1];
			}
		}
		/// <summary>
		/// 激活窗口数量
		/// </summary>
		public static int activeCount => msUsedWindows.Count;
		/// <summary>
		/// 打开窗口事件
		/// </summary>
		public static event Action<Window> onOpenWindow;
		/// <summary>
		/// 关闭窗口事件
		/// </summary>
		public static event Action<Window> onCloseWindow;

		/// <summary>
		/// 获取Order最大的窗口
		/// </summary>
		public static Window topWindow
		{
			get
			{
				if (msUsedWindows.Count > 0)
				{
					return msUsedWindows.Max();
				}
				return null;
			}
		}

		/// <summary>
		/// 节点
		/// </summary>
		public static Transform root
		{
			get
			{
				if (Get() == null) return null;
				return Get().transform;
			}
		}

		/// <summary>
		/// UI相机
		/// </summary>
		public static Camera uiCamera
		{
			get
			{
				if (Get() == null) return null;
				return Get().mUICamera;
			}
		}


		private Camera mUICamera;
		private Canvas mMaskCanvas;

		public override ELifetime lifetime => ELifetime.App;

		public string systemName => typeof(WindowManager).Name;

		public IModuleInterface module { get; set; }

		#region 实例方法
		protected override void OnInitialize()
		{
			base.OnInitialize();
			onOpenWindow -= WindowFadeIn;
			onOpenWindow += WindowFadeIn;
			onCloseWindow -= WindowFadeOut;
			onCloseWindow += WindowFadeOut;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			onOpenWindow -= WindowFadeIn;
			onCloseWindow -= WindowFadeOut;
			DestroyAll();
		}

		private IEnumerator OnLoad()
		{
			while (true)
			{
				if (msWaitingLoadQueue.Count <= 0)
				{
					yield return null;
				}
				else
				{
					var item = msWaitingLoadQueue.Dequeue();
					yield return item;
				}
			}
		}

		private void StopFade()
		{
			mMaskCanvas.gameObject.SetActive(false);
		}

		private void WindowFadeIn(Window win)
		{
			int order = maskOrder;
			mMaskCanvas.sortingOrder = order - 2;
			if (order <= 0) return;
			mMaskCanvas.gameObject.SetActive(true);
		}

		private void WindowFadeOut(Window win)
		{
			int order = maskOrder;
			mMaskCanvas.sortingOrder = order - 2;
			if (order > 0) return;
			mMaskCanvas.gameObject.SetActive(false);
		}
		#endregion

		#region 操作方法
		/// <summary>
		/// 加载Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="onLoaded"></param>
		private static void LoadWindow<T>(Action<T> onLoaded) where T : Window
		{
			string windowName = typeof(T).Name;
			msWaitingLoadQueue.Enqueue(LoadWindow(windowName, (Action<GameObject, T>)((go, win) =>
			{
				if (go == null || win == null)
				{
					DebugUtility.LogError(LoggerTags.UI, "加载Window失败，Name:{0}", windowName);
					Misc.SafeInvoke(onLoaded, null);
					return;
				}
				var trans = go.transform;
				trans.name = windowName;
				trans.SetParent(root);
				win.Initialize((Camera)WindowManager.uiCamera);
				msUnusedWindows.Add(win);
				win.SetActive(false);
				Misc.SafeInvoke(onLoaded, win);
			})));
		}

		/// <summary>
		/// 获取或者加载未使用的Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="onCallback"></param>
		private static void GetOrLoadWindow<T>(Action<T> onCallback) where T : Window
		{
			T win = GetUnusedWindow<T>();
			if (win != null && !win.HasDestroyed())
			{
				Misc.SafeInvoke(onCallback, win);
			}
			else
			{
				LoadWindow(onCallback);
			}
		}

		/// <summary>
		/// 获取未使用的Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static T GetUnusedWindow<T>() where T : Window
		{
			string typeName = typeof(T).Name;
			for (int i = msUnusedWindows.Count - 1; i >= 0; i--)
			{
				Window win = msUnusedWindows[i];
				if (win == null || win.HasDestroyed()) continue;
				if (win.name.Equals(typeName)) return win as T;
			}
			return default(T);
		}

		/// <summary>
		/// 使用中的Windows进行渲染排序
		/// </summary>
		private static void SortUsedWindows()
		{
			msMaskOrderMapper.Clear();
			for (int i = 0; i < msUsedWindows.Count; i++)
			{
				Window win = msUsedWindows[i];
				if (win == null || win.HasDestroyed()) continue;
				win.SetOrder(i * msDeltaOrder);
				if (win.hasMask)
				{
					msMaskOrderMapper.Add(win.order);
				}
			}
			msMaskOrderMapper.Sort((a, b) => a.CompareTo(b));
		}

		/// <summary>
		/// 清除脏数据
		/// </summary>
		private static void ClearDirty()
		{
			msWaitingLoadQueue.Clear();
			msUnusedWindows.Clear();
			msUsedWindows.Clear();
			msMaskOrderMapper.Clear();
			if (Get() == null) return;
			Get().StopFade();
		}
		#endregion

		#region  内部操作
		private static bool DoOpen(Window win, object param)
		{
			try
			{
				if (win == null || win.HasDestroyed()) return false;
				DebugUtility.Log(LoggerTags.UI, "DoOpen : {0}", win.name);

				if (win.used)
				{
					msUsedWindows.Remove(win);
				}
				else
				{
					msUnusedWindows.Remove(win);
				}

				msUsedWindows.Add(win);
				DebugUtility.Log(LoggerTags.UI, "Before SortUsedWindows : {0}", win.name);
				SortUsedWindows();
				DebugUtility.Log(LoggerTags.UI, "After SortUsedWindows : {0}", win.name);
				win.Open(param);
				DebugUtility.Log(LoggerTags.UI, "After Open : {0}", win.name);
				Misc.SafeInvoke(onOpenWindow, win);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
				return false;
			}
			return true;
		}

		private static bool DoClose(Window win)
		{
			if (win == null || win.HasDestroyed()) return false;

			msUsedWindows.Remove(win);
			msUnusedWindows.Add(win);
			win.Close();
			if (win.hasMask) msMaskOrderMapper.Remove(win.order);
			Misc.SafeInvoke(onCloseWindow, win);
			return true;
		}

		private static bool DoDestroy(Window win)
		{
			if (win == null || win.HasDestroyed()) return false;

			if (win.used)
			{
				msUsedWindows.Remove(win);
				win.Destroy();
				if (win.hasMask) msMaskOrderMapper.Remove(win.order);
				Misc.SafeInvoke(onCloseWindow, win);
			}
			else
			{
				msUnusedWindows.Remove(win);
			}

			UnloadWindow(win);
			return true;
		}
		#endregion

		#region 外部接口
		/// <summary>
		/// 获取使用中的Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetUsedWindow<T>() where T : Window
		{
			string typeName = typeof(T).Name;
			for (int i = msUsedWindows.Count - 1; i >= 0; i--)
			{
				Window win = msUsedWindows[i];
				if (win == null || win.HasDestroyed()) continue;
				if (win.name.Equals(typeName)) return win as T;
			}
			return default(T);
		}

		/// <summary>
		/// 打开Window(每次打开都是唯一窗口，只存在一个同种类型窗口)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		/// <param name="onOpen"></param>
		public static void Open<T>(object param = null, Action<T> onOpen = null) where T : Window
		{
			DebugUtility.Log(LoggerTags.UI, "Open window : {0}", typeof(T).Name);
			T win = GetUsedWindow<T>();
			DebugUtility.Log(LoggerTags.UI, "After get used window : {0}", typeof(T).Name);
			if (win != null)
			{
				DoOpen(win, param);
				DebugUtility.Log(LoggerTags.UI, "After window opened : {0}", typeof(T).Name);
				Misc.SafeInvoke(onOpen, win);
				DebugUtility.Log(LoggerTags.UI, "After window open callback : {0}", typeof(T).Name);
			}
			else
			{
				DebugUtility.Log(LoggerTags.UI, "Before GetOrLoadWindow : {0}", typeof(T).Name);

				GetOrLoadWindow<T>((w) =>
				{
					DoOpen(w, param);
					DebugUtility.Log(LoggerTags.UI, "After window opened : {0}", typeof(T).Name);
					Misc.SafeInvoke(onOpen, w);
					DebugUtility.Log(LoggerTags.UI, "After window open callback : {0}", typeof(T).Name);
				});
			}

		}

		/// <summary>
		/// 新建打开Window(每次打开都是新建窗口，允许存在多个同种类型窗口)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		/// <param name="onOpen"></param>
		public static void NewOpen<T>(object param = null, Action<T> onOpen = null) where T : Window
		{
			GetOrLoadWindow<T>((w) =>
			{
				DoOpen(w, param);
				Misc.SafeInvoke(onOpen, w);
			});
		}

		/// <summary>
		/// 关闭Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool Close<T>() where T : Window
		{
			T win = GetUsedWindow<T>();
			return DoClose(win);
		}

		/// <summary>
		/// 关闭Window
		/// </summary>
		/// <param name="win"></param>
		/// <returns></returns>
		public static bool Close(Window win)
		{
			return DoClose(win);
		}

		/// <summary>
		/// 关闭所有Windows
		/// </summary>
		public static void CloseAll()
		{
			for (int i = msUsedWindows.Count - 1; i >= 0; i--)
			{
				Window win = msUsedWindows[i];
				DoClose(win);
			}
			msUsedWindows.Clear();
			msMaskOrderMapper.Clear();
		}

		/// <summary>
		/// 销毁Window
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool Destroy<T>() where T : Window
		{
			T win = GetUsedWindow<T>() ?? GetUnusedWindow<T>();
			return DoDestroy(win);
		}

		/// <summary>
		/// 销毁所有Windows
		/// </summary>
		public static void DestroyAll()
		{
			for (int i = msUsedWindows.Count - 1; i >= 0; i--)
			{
				Window win = msUsedWindows[i];
				DoDestroy(win);
			}

			for (int i = msUnusedWindows.Count - 1; i >= 0; i--)
			{
				Window win = msUnusedWindows[i];
				DoDestroy(win);
			}
			ClearDirty();
		}

		public IEnumerator Initialize()
		{
			StopAllCoroutines();
			const string key = "WindowSystem";
			return LoadPrefab(key, (go) =>
			{
				if (go == null) return;
				go.name = key;
				var trans = go.transform;
				trans.SetParent(transform, false);
				mUICamera = go.GetComponent<Camera>("UICamera");
				mMaskCanvas = go.GetComponent<Canvas>("MaskCanvas");
			});
		}

		public IEnumerator PostInitialize()
		{
			// keep running
			StartCoroutine(OnLoad());

			yield break;
		}

		public void Uninitialize()
		{
		}

		public void Startup()
		{
		}

		public void Shutdown()
		{
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnUpdate(float deltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}
		#endregion
	}
}
