using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Loki;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Ubtrobot
{
	public partial class RobotFactory : USingletonObject<RobotFactory>
	{
		public void LoadRobot(LitJson.JsonData jsonData, Action<IRobot> onCompleted)
		{
			if (jsonData == null)
			{
				Misc.SafeInvoke(onCompleted, null);
				return;
			}
			IRobot robot = null;
			bool hideLines = Application.isPlaying;
			try
			{
				ProfilingUtility.BeginSample("RobotFactory.LoadRobot");
				DefaultRobotSerializer defaultRobotSerializer = new DefaultRobotSerializer();
				var dataModel = defaultRobotSerializer.FromJson(jsonData);
				if (dataModel != null)
				{
					ProfilingUtility.BeginSample("RobotFactory.LoadRobot.PrepareParts");
					var parts = new Dictionary<string, Queue<GameObject>>(dataModel.assetDatas.Count);
					foreach (var item in dataModel.assetDatas)
					{
						string resID = item.Key;
						// 去掉线
						if (hideLines && resID.StartsWith("W") && resID.IndexOf('_') > 0)
						{
							continue;
						}

						var count = item.Value;
						while (count > 0)
						{
							var part = UbtrobotSettings.GetOrLoad().partsLibrary.Instantiate(resID);
							if (part == null)
							{
								DebugUtility.LogError(LoggerTags.Project, "Missing part : {0}", resID);
								break;
							}

							if (hideLines && resID == "Battery")
							{
								part.transform.SetActive("Battery_02", false);
							}

							if (!parts.TryGetValue(resID, out var list))
							{
								list = new Queue<GameObject>();
								parts[resID] = list;
							}
							count--;
							list.Enqueue(part.gameObject);
						}
					}
					ProfilingUtility.EndSample();
					ProfilingUtility.BeginSample("RobotFactory.LoadRobot.Rebuild");
					var robotTransform = dataModel.Rebuild(null, parts);
					ProfilingUtility.EndSample();
					robot = robotTransform != null ? robotTransform.GetComponent<IRobot>() : null;
					if (robot != null)
					{
						if (Application.isPlaying)
							RobotManager.GetOrAlloc().AddRobot(robot);
						else
							robot.AutoInitialize();
					}
				}
				ProfilingUtility.EndSample();
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			Misc.SafeInvoke(onCompleted, robot);
		}

		public void LoadRobot(string jsonData, Action<IRobot> onCompleted)
		{
			try
			{
				LoadRobot(JsonMapper.ToObject(jsonData), onCompleted);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		public void LoadRobot(byte[] jsonData, Action<IRobot> onCompleted)
		{
			try
			{
				var jsonString = System.Text.UTF8Encoding.UTF8.GetString(jsonData);
				LoadRobot(JsonMapper.ToObject(jsonString), onCompleted);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}


		[ConsoleMethod(aliasName = "robot.loadRobot")]
		public void LoadRobotFromResources(string path)
		{
			LoadRobotFromResources(path, null);
		}

		public void LoadRobotFromResources(string path, Action<IRobot> onCompleted)
		{
			var resQ = AssetManager.LoadFromResourcesAsync<TextAsset>(path);
			if (resQ != null)
			{
				resQ.completed += (ao) =>
				{
					bool fallback = true;
					var req = ao as ResourceRequest;
					if (req != null && (req.asset is TextAsset textAsset))
					{
						var text = textAsset.text;
						if (!string.IsNullOrEmpty(text))
						{
							LoadRobot(text, onCompleted);
							fallback = false;
						}
						AssetManager.UnloadAsset(req.asset);
					}

					if (fallback)
					{
						Misc.SafeInvoke(onCompleted, null);
					}
				};
				return;
			}
			Misc.SafeInvoke(onCompleted, null);
		}

	}

#if UNITY_EDITOR
	public partial class RobotFactory
	{
		[AssetPathToObject]
		[SerializeField]
		private string testLoadRobot;

		[PreviewMember]
		public bool enablePartsPrefabLink
		{
			get
			{
				return UbtrobotSettings.GetOrLoad().partsLibrary.enablePartsPrefabLink;
			}
			set
			{
				UbtrobotSettings.GetOrLoad().partsLibrary.enablePartsPrefabLink = value;
			}
		}

		[InspectorMethod]
		public void LocateInExportePath()
		{
			var labModelExportPath = UbtrobotSettings.GetOrLoad().pathSettings.GetExportAssetPath();
			//var lastActive = UnityEditor.Selection.activeObject;
			UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath(labModelExportPath, typeof(DefaultAsset));
			//UnityEditor.Selection.activeObject = lastActive;
		}

		[InspectorMethod]
		public void LoadTestRobot()
		{
			if (string.IsNullOrEmpty(testLoadRobot))
				return;
			var text = AssetDatabase.LoadAssetAtPath<TextAsset>(testLoadRobot);
			LoadRobot(text.text, null);
		}
	}
#endif

}
