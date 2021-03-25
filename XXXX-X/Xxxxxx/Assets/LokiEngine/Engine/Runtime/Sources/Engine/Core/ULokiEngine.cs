using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Loki
{
	[UCLASS(config = "Engine")]
	public sealed class ULokiEngine : UEngine, IEngine
	{
		private static bool msInitialized = false;
		private static ULokiEngine msEngine;
		private IEngineLoop mEngineLoop;

		public bool initialized { get { return mEngineLoop != null && mEngineLoop.initialized; } }

		public static Action<IEngine> onEngineInitialized = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Startup()
		{
			ProfilingUtility.BeginSample("ULokiEngine.Startup");
			Get();
			ProfilingUtility.EndSample();
		}

		private static void Registers()
		{
			if (!msInitialized)
			{
				msInitialized = true;
				GlobalReflectionCache.LoadAssemblies();
				NameToTypeUtility.Initialize();
				var allModules = GlobalReflectionCache.FindTypes<ModuleAttribute>(false);
				if (allModules != null)
				{
					allModules.Sort(EngineComparers.defaultModuleAttributeComparer);
					foreach (var m in allModules)
					{
						//DebugUtility.Log(LoggerTags.Engine, "Module {0} Loaded.", m);
						ModuleAttribute attr = m.GetCustomAttribute<ModuleAttribute>(true);
						if (!string.IsNullOrEmpty(attr.entryPoint))
						{
							var entry = m.GetMethod(attr.entryPoint, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
							if (entry != null)
							{
								//DebugUtility.Log(LoggerTags.Engine, "Module {0} Entry.", m);
								entry.Invoke(null, null);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Do not call Get() in OnDisable() and OnDestroy(), otherwise the UnityEditor will generate some unexpect GameObject
		/// </summary>
		/// <returns></returns>
		public static ULokiEngine Get()
		{
			if (msEngine == null)
			{
				msEngine = FindObjectOfType<ULokiEngine>();
				if (msEngine != null)
					msEngine.AsImmortal(typeof(LokiEngineModule).Name);
			}
			if (msEngine == null)
			{
				msEngine = NewImmortalObject<ULokiEngine>(typeof(ULokiEngine).Name, typeof(LokiEngineModule).Name);
			}
			if (msEngine != null)
			{
				msEngine.enabled = true;
			}
#if UNITY_EDITOR
			Registers();
#endif
			return msEngine;
		}

		private new void Awake()
		{
			base.Awake();
			mEngineLoop = new EngineLoop(this);
			PreloadManager.GetOrAlloc();

			DebugUtility.Log(LoggerTags.Engine, "EngineLoop Start Frame : {0}", Time.frameCount);
			//DebugUtility.Log(LoggerTags.Engine, "EngineLoop End Frame : {0}", Time.frameCount);
		}

		private IEnumerator Start()
		{
			if (this.HasDestroyed())
				yield break;

			Registers();
			yield return StartCoroutine(mEngineLoop.Initialize(this));
		}

		private void OnDisable()
		{
			if (this.HasDestroyed())
				return;

			// DebugUtility.LogTrace(LoggerTags.Engine, "Why disable engine.");
		}

		private void FixedUpdate()
		{
			if (this.HasDestroyed())
				return;

			mEngineLoop.FixedUpdate(Time.fixedDeltaTime);
		}

		private void Update()
		{
			if (this.HasDestroyed())
				return;
			if (onEngineInitialized != null)
			{
				var init = onEngineInitialized;
				onEngineInitialized = null;
				Misc.SafeInvoke(init, this);
			}
			mEngineLoop.Update(Time.deltaTime);
		}

		private void LateUpdate()
		{
			if (this.HasDestroyed())
				return;

			mEngineLoop.LateUpdate();
		}

		protected override void OnDestroy()
		{
			if (this.HasDestroyed())
				return;

			DebugUtility.Log(LoggerTags.Engine, "Engine::OnDestroy");
			base.OnDestroy();
			mEngineLoop.Dispose();
			mEngineLoop = null;
		}

		/// <summary>
		/// OnApplicationQuit is called before OnDestroy (Unity Engine Feature)
		/// <param>
		/// Decommissioning(OnApplicationQuit -> OnDisable -> OnDestroy)
		/// </param>
		/// <param>
		/// Please do not run coroutine after 'OnApplicationQuit', it is not working.
		/// </param>
		/// </summary>
		protected override void OnApplicationQuit()
		{
			if (this.HasDestroyed())
				return;

			DebugUtility.Log(LoggerTags.Engine, "Engine::OnApplicationQuit");
			base.OnApplicationQuit();
			mEngineLoop.OnApplicationQuit();
			//hasQuitApplication = true;
		}

	}
}
