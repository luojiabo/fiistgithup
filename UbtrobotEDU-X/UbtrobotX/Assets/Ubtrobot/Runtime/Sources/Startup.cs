using System.Collections;
using DG.Tweening.Plugins.Core.PathCore;
using Loki;
using Loki.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ubtrobot
{
	[ExecuteInEditMode]
	public class Startup : UObject
	{
		[SceneName]
		public string sceneName = "";

		[AssetPathToObject]
		public string modelAssetName = "";

		protected void Start()
		{
			DebugUtility.LogTrace("Startup", "Startup");

			ULokiEngine.onEngineInitialized = OnEngineInitialized;
		}

		private void OnEngineInitialized(IEngine engine)
		{
			DebugUtility.LogTrace(LoggerTags.Engine, "OnEngineInitialized");
			var currentScene = SceneManager.GetActiveScene();

			// temp code
			if (!string.IsNullOrEmpty(modelAssetName))
			{
				LoadModel(FileSystem.AnyPathToResourcesPath(modelAssetName, true), true);
				return;
			}

			if (sceneName != currentScene.name)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
			}
			else
			{
				SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
			}
		}

		public static void LoadModel(string modelID, bool firstInitial = false)
		{
			if (!string.IsNullOrEmpty(modelID))
			{
				// temp code
				if (modelID.IndexOf('/') < 0)
				{
					modelID = string.Concat("DriveProject/Drive/", System.IO.Path.GetFileNameWithoutExtension(modelID));
				}

				if (!firstInitial)
					WindowManager.CloseAll();
				SceneManager.LoadSceneAsync("Drive", LoadSceneMode.Single).completed += (h) =>
				{
					var resQ = AssetManager.LoadFromResourcesAsync<GameObject>(modelID);
					if (resQ != null)
					{
						resQ.completed += (ao) =>
						{
							var res = ao as ResourceRequest;
							if (res != null && res.asset != null)
							{
								var drive = Instantiate<GameObject>((GameObject)res.asset);
								drive.name = res.asset.name;
							}
							if (!firstInitial)
								WindowManager.Open<SimulationWindow>();
						};
					}
				};
			}
		}

		public static void FirstLoad(string modelID)
		{
			if (!string.IsNullOrEmpty(modelID))
			{
				// temp code
				if (modelID.IndexOf('/') < 0)
				{
					modelID = string.Concat("DriveProject/Drive/", System.IO.Path.GetFileNameWithoutExtension(modelID));
				}

				SceneManager.LoadSceneAsync("Drive", LoadSceneMode.Single).completed += (h) =>
				{
					var resQ = AssetManager.LoadFromResourcesAsync<GameObject>(modelID);
					if (resQ != null)
					{
						resQ.completed += (ao) =>
						{
							var res = ao as ResourceRequest;
							if (res != null && res.asset != null)
							{
								var drive = Instantiate<GameObject>((GameObject)res.asset);
								drive.name = res.asset.name;
							}
						};
					}
				};
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		[ConsoleMethod(aliasName = "p.Startup")]
		[ContextMenu("Print")]
		private void Print()
		{
			DebugUtility.LogError(LoggerTags.Project, "PrintStarup");
		}

		private void Reset()
		{
			name = typeof(Startup).Name;
		}
	}
}
