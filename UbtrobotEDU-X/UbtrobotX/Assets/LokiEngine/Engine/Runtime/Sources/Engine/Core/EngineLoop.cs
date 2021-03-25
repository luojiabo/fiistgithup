using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Loki
{
	internal sealed class EngineLoop : IEngineLoop
	{
		private IEngine mEngine;
		private bool mHasDisposed = false;

		public bool initialized { get; private set; } = false;

		public EngineLoop(IEngine currentEngine)
		{
			mEngine = currentEngine;
		}

		~EngineLoop()
		{
			Dispose(false);
		}

		public IEnumerator Initialize(UEngine engine)
		{
			yield return engine.StartCoroutine(InitializeAsync(engine));
		}

		private IEnumerator InitializeAsync(UEngine engine)
		{
			DebugUtility.Log(LoggerTags.Engine, "EngineLoop.InitializeAsync: {0}", Time.realtimeSinceStartup);
			if (!initialized)
			{
				// Initialize the preload manager
				yield return engine.StartCoroutine(PreloadManager.GetOrAlloc().Initialize());
				// apply engine settings
				var engineConfig = EngineSettings.GetOrLoad();
				if (engineConfig != null)
					DebugUtility.Initialize(engineConfig.loggerSettings);
				// Initialize the modules
				yield return engine.StartCoroutine(ModuleManager.Get().OnModuleInitialize(engine));

				initialized = true;
			}

			DebugUtility.Log(LoggerTags.Engine, "EngineLoop.InitializeAsync done: {0}", Time.realtimeSinceStartup);
		}

		public void FixedUpdate(float fixedDeltaTime)
		{
			// Do FixedUpdate in the modules
			ModuleManager.Get().FixedUpdate(fixedDeltaTime);
		}

		public void Update(float deltaTime)
		{
			// Do Update in the modules
			ModuleManager.Get().Update(deltaTime);
		}

		public void LateUpdate()
		{
			// Do LateUpdate in the modules
			ModuleManager.Get().OnLateUpdate();
		}

		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (mHasDisposed)
				return;

			mHasDisposed = true;
			OnDispose(disposing);
		}

		public void OnApplicationQuit()
		{
			ModuleManager.Get().OnApplicationQuit();
		}

		private void OnDispose(bool disposing)
		{
			if (disposing)
			{
				ModuleManager.Get().Uninitialize();
			}
		}

		public void ClearPendingCleanupObjects()
		{
			ModuleManager.Get().ClearPendingCleanupObjects();
		}
	}
}
