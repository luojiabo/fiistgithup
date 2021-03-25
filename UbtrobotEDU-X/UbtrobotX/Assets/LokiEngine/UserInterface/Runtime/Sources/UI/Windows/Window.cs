using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Loki.UI
{
	/// <summary>
	/// Window显示层
	/// </summary>
	public enum WindowLayer
	{
		Background,//主界面等常驻的界面
		Forgeground,//游戏具体功能界面等
		TopMost,//场景切换界面，提示界面占据
	}

	/// <inheritdoc />
	/// <summary>
	/// 窗口基类（Window > Panel > Widget）
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(CanvasScaler))]
	[RequireComponent(typeof(GraphicRaycaster))]
	public abstract class Window : WidgetContainer, IComparable<Window>
	{
		private static readonly Dictionary<WindowLayer, int> msLayerToOrder = new Dictionary<WindowLayer, int>()
		{
			{WindowLayer.Background, 50},
			{WindowLayer.Forgeground, 150},
			{WindowLayer.TopMost, 250},
		};
		/// <summary>
		/// 是否已经初始化
		/// </summary>
		private bool mInited = false;
		/// <summary>
		/// 界面Canvas
		/// </summary>
		private Canvas mCanvas;
		/// <summary>
		/// 窗口默认SortingOrder
		/// </summary>
		private int defaultOrder { get { return msLayerToOrder[layer]; } }
		/// <summary>
		/// 内容节点
		/// </summary>
		public RectTransform contentNode { get; private set; }
		/// <summary>
		/// 窗口当前SortingOrder
		/// </summary>
		public int order { get { return mCanvas.sortingOrder; } }
		/// <summary>
		/// 是否被使用
		/// </summary>
		public bool used { get; private set; }
		/// <summary>
		/// 窗口遮罩
		/// </summary>
		public bool hasMask = false;
		/// <summary>
		/// 窗口显示层
		/// </summary>
		public WindowLayer layer = WindowLayer.Forgeground;
		/// <summary>
		/// 所属Canvas
		/// </summary>
		public Canvas canvas => mCanvas;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!mInited) return;
			OnUninit();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="rendererCamera"></param>
		public void Initialize(Camera rendererCamera)
		{
			try
			{
#if UNITY_EDITOR
				gameObject.name = GetType().Name;
#endif
				mCanvas = GetComponent<Canvas>();
				mCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				mCanvas.worldCamera = rendererCamera;
				mCanvas.sortingOrder = defaultOrder;

				contentNode = transform.GetComponent<RectTransform>("Root/Content");
				OnInit();
				mInited = true;
			}
			catch (Exception ex)
			{
				DebugUtility.LogErrorTrace(LoggerTags.UI, "Error when initializing {0}", GetType().Name);
				DebugUtility.LogException(ex);
			}
		}

		/// <summary>
		/// 设置sortingOrder
		/// </summary>
		/// <param name="offset"></param>
		public void SetOrder(int offset)
		{
			mCanvas.sortingOrder = defaultOrder + offset;
		}

		/// <summary>
		/// 打开
		/// </summary>
		public void Open(object param)
		{
			used = true;
			SetActive(used);
			OnOpen(param);
		}
		protected abstract void OnOpen(object param);

		/// <summary>
		/// 关闭
		/// </summary>
		public void Close()
		{
			used = false;
			SetActive(used);
			OnClose();
		}
		protected virtual void OnClose() { }

		/// <summary>
		/// 返回
		/// </summary>
		public virtual bool Back() { return false; }

		/// <summary>
		/// 销毁
		/// </summary>
		public void Destroy()
		{
			used = false;
			SetActive(used);
			OnClose();
		}

		public int CompareTo(Window other)
		{
			return order.CompareTo(other.order);
		}
	}
}
