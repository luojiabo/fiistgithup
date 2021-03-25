using System;
using System.Collections;
using Loki;
using Loki.UI;
using UnityEngine.SceneManagement;

namespace Ubtrobot
{
	public class TestSystem : USingletonObject<TestSystem>, ISystem
	{
		private static readonly Type msType = typeof(TestSystem);

		private AssetManagerTest mAssetMgrTest = new AssetManagerTest();

		public string systemName => msType.Name;

		public IModuleInterface module { get; set; }

		public IEnumerator Initialize()
		{
			mAssetMgrTest.RegisterToConsole();
			yield break;
		}

		public IEnumerator PostInitialize()
		{
			var scene = SceneManager.GetActiveScene();
			if (scene != null && scene.GetRootGameObjects().Length == 0)
			{
				SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
			}
			return null;
		}

		public void Uninitialize()
		{
			mAssetMgrTest.UnregisterFromConsole();
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}

		public void OnUpdate(float deltaTime)
		{
		}

		public void Register()
		{
		}

		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		[ConsoleMethod(aliasName = "test.loadScene")]
		public void LoadScene(string sceneID)
		{
			if (!string.IsNullOrEmpty(sceneID))
			{
				WindowManager.CloseAll();
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneID, UnityEngine.SceneManagement.LoadSceneMode.Single).completed += (h) =>
				{
					WindowManager.Open<SimulationWindow>();
				};
			}
		}

		[ConsoleMethod(aliasName = "test.loadModel")]
		public void LoadModel(string modelID)
		{
			Ubtrobot.Startup.LoadModel(modelID);
		}

		[ConsoleMethod(aliasName = "test.loadBuildIndex")]
		public void LoadBuildIndex(int buildIndex)
		{
			if (buildIndex >= 0)
			{
				WindowManager.CloseAll();
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex, UnityEngine.SceneManagement.LoadSceneMode.Single).completed += (h) =>
				{
					WindowManager.Open<SimulationWindow>();
				};
			}
		}
	}
}
