using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Loki
{
	public delegate void OnScreenSizeChanged(int width, int height);

	[DebuggerDisplay("w x h ({mWidth} x {mHeight})")]
	public class ScreenSystem : USingletonObject<ScreenSystem>, ISystem
	{
		private static readonly Type msType = typeof(ScreenSystem);

		public static event OnScreenSizeChanged screenSizeChanged;

		private int mWidth = 0;
		private int mHeight = 0;
		private bool mIsUpdate = false;

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		public IEnumerator Initialize()
		{
			mWidth = Screen.width;
			mHeight = Screen.height;

			DebugUtility.Log(LoggerTags.Engine, "Initialize Screen size : {0}", ToString());
			yield break;
		}

		public IEnumerator PostInitialize()
		{
			return null;
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}

		private void CheckScreenSizeChanged()
		{
			if (mWidth != Screen.width || mHeight != Screen.height)
			{
				mWidth = Screen.width;
				mHeight = Screen.height;

				//DebugUtility.Log(LoggerTags.Engine, "On Screen size changed : {0} x {1} (width x height)", mWidth, mHeight);

				if (screenSizeChanged != null)
				{
					screenSizeChanged(mWidth, mHeight);
				}
			}
		}

		public void OnUpdate(float deltaTime)
		{
			if (mIsUpdate)
				return;
			mIsUpdate = true;
			try
			{
				CheckScreenSizeChanged();
			}
			catch(Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			finally
			{
				mIsUpdate = false;
			}
		}


		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		public void Uninitialize()
		{
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "Screen.Info")]
		public override string ToString()
		{
			return 
				string.Concat(
					"[", mWidth.ToString(), " x ", mHeight.ToString(), "](w x h)", 
					".\nCurrentResolution : ", Screen.currentResolution.ToString(), 
					"\nOrientation : ", Screen.orientation.ToString(),
					"\nFullscreen : ", Screen.fullScreen.ToString(),
					"\nDPI : ", Screen.dpi.ToString(),
					"\nSleepTimeout : ", Screen.sleepTimeout.ToString());
		}
	}
}
