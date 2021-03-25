using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Loki
{
	/// <summary>
	/// Performance analytics for Loki Engine
	/// CPU/GPU/Memory/FPS
	/// a.k.a "pref."
	/// All of its console members' alias names must start with "pref."
	/// <seealso cref="PerformanceAnalytics.visible"/>
	/// </summary>
	public class PerformanceAnalytics : USingletonObject<PerformanceAnalytics>, ISystem
	{
		private static readonly Type msType = typeof(PerformanceAnalytics);

		private bool mIsVisible = false;

		private readonly FPSCounter mFPSCounter = new FPSCounter();

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

		[ConsoleProperty(aliasName = "perf.visible")]
		public bool visible
		{
			get { return mIsVisible && isActiveAndEnabled; }
			set
			{
				if (mIsVisible != value)
				{
					mIsVisible = value;
					if (mIsVisible)
					{

					}
				}
			}
		}

		[ConsoleProperty(aliasName = "perf.fps")]
		public bool fps { get; set; } = true;

		public IEnumerator Initialize()
		{
			mFPSCounter.OnInitialize();
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

		public void OnUpdate(float deltaTime)
		{
			mFPSCounter.OnUpdate(deltaTime);
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

		private void OnGUI()
		{
			float yStartOffset = 5.0f;
			if (fps)
			{
				var fpsSize = mFPSCounter.size;
				Rect fpsPos = new Rect(ApplicationUtility.screenWidth - fpsSize.x, yStartOffset, fpsSize.x, fpsSize.y);
				mFPSCounter.OnGUI(fpsPos);
				yStartOffset += fpsSize.y;
			}

				
		}
	}
}
