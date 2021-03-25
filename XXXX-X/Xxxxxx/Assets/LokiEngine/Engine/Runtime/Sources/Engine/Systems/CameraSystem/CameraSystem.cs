#define POSTPROCESSING_OUTLINE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Loki
{
	[DebuggerDisplay("activeControllerName = \"{activeControllerName}\"")]
	public abstract class CameraSystem : ISystem
	{
		protected readonly TDynamicArray<Camera> mCameras = new TDynamicArray<Camera>(4);
		protected readonly List<ICameraController> mControllers = new List<ICameraController>();
		protected readonly UpdateHandler mUpdator = new UpdateHandler();

		private bool mEnableOutline = false;
		private ICameraController mActiveController = null;

		public Action<ICameraController> onCameraDisableEvent;
		public Action<ICameraController> onCameraEnableEvent;

		public string systemName { get { return "CameraSystem"; } }

		public IModuleInterface module { get; set; }

		public ICameraController activeController
		{
			get => mActiveController;
			private set
			{
				if (mActiveController != value)
				{
					if (mActiveController != null)
						OnCameraDisable(mActiveController);
					mActiveController = value;
					if (mActiveController != null)
						OnCameraEnable(mActiveController);
				}
			}
		}

		public string activeControllerName { get { return activeController != null ? activeController.name : string.Empty; } }


		public void SetOutline(bool enable)
		{
#if POSTPROCESSING_OUTLINE
			if (activeController != null)
			{
				if (enable)
				{
					// todo Transform => Outline
					var outlinePP = activeController.transform.GetOrAllocComponent<OutlineEffect>();
					outlinePP.enabled = true;

				}
				else
				{
					var outlinePP = activeController.transform.GetComponent<OutlineEffect>();
					if (outlinePP != null)
						outlinePP.enabled = false;
				}
			}
#endif
		}

		private void OnCameraDisable(ICameraController controller)
		{
			SetOutline(false);

			Misc.SafeInvoke(onCameraDisableEvent, controller);
		}

		private void OnCameraEnable(ICameraController controller)
		{
			CheckOutline();
			Misc.SafeInvoke(onCameraEnableEvent, controller);
		}

		public void CheckOutline()
		{
#if POSTPROCESSING_OUTLINE
			var currentCtrl = activeController;
			if (currentCtrl != null)
			{
				var outlines = OutlineStorage.Instance().OutlinesForKey(currentCtrl.postProcessingLayerMask);
				var enableOutline = outlines != null && outlines.Count > 0;
				SetOutline(enableOutline);
			}
#endif

		}

		public IEnumerator Initialize()
		{
			return null;
		}

		public IEnumerator PostInitialize()
		{
			return null;
		}

		public virtual void OnFixedUpdate(float fixedDeltaTime)
		{
			mUpdator.OnFixedUpdate(fixedDeltaTime);
		}

		public virtual void OnUpdate(float deltaTime)
		{
			mUpdator.OnUpdate(deltaTime);
		}

		public virtual void OnLateUpdate()
		{
			mUpdator.OnLateUpdate();
		}


		public virtual void Shutdown()
		{
		}

		public virtual void Startup()
		{
		}

		public virtual void Uninitialize()
		{
		}

		public virtual void AddController(ICameraController c, bool active = false)
		{
			if (mControllers.Contains(c))
			{
				return;
			}

			mControllers.Add(c);
			mUpdator.Add(c);

			if (active)
			{
				DoActiveController(c);
			}
			else if (c.active)
			{
				c.active = false;
			}
		}

		public bool DoActiveController(ICameraController c)
		{
			if (c != null)
			{
				if (activeController == c)
				{
					return true;
				}

				var lastActived = activeController;
				activeController = c;
				if (!activeController.active)
				{
					activeController.active = true;
				}
				if (lastActived != null && c != lastActived && lastActived.active)
				{
					lastActived.active = false;
				}
				return true;
			}

			return false;
		}

		public virtual bool ActiveController(ICameraController c)
		{
			return DoActiveController(c);
		}

		public virtual bool DeactiveController(ICameraController c)
		{
			return DoActiveController(c);
		}

		public virtual bool ActiveController(string name)
		{
			return ActiveController(mControllers.Find(temp => temp.name == name));
		}

		public virtual bool ActiveControllerByTag(string tag)
		{
			return ActiveController(mControllers.Find(temp => temp.CompareTag(tag)));
		}

		public virtual void RemoveController(string name, bool autoActiveLast = true)
		{
			DoRemoveController(mControllers.Find(temp => temp.name == name), autoActiveLast);
		}

		protected void DoRemoveController(ICameraController c, bool autoActiveLast)
		{
			mUpdator.Remove(c);
			if (mControllers.Remove(c))
			{
				if (activeController == c)
				{
					activeController = null;

					if (mControllers.Count > 0 && autoActiveLast)
					{
						activeController = mControllers.Last();
						if (!activeController.active)
						{
							activeController.active = true;
						}
					}
				}

				if (c.active)
				{
					c.active = false;
				}
			}
		}

		public virtual void RemoveController(ICameraController c, bool autoActiveLast = true)
		{
			DoRemoveController(c, autoActiveLast);
		}

		public TDynamicArray<Camera> GetAllCameras(bool force = false)
		{
			if (mCameras.count == 0 || force)
			{
				mCameras.Request(Camera.allCamerasCount, true);
				Camera.GetAllCameras(mCameras.Ref());
			}
			return mCameras;
		}
	}

	public abstract class CameraSystem<TMostDerived> : CameraSystem where TMostDerived : CameraSystem<TMostDerived>
	{
	}

	public sealed class DefaultCameraSystem : Loki.CameraSystem<DefaultCameraSystem>
	{

	}
}
