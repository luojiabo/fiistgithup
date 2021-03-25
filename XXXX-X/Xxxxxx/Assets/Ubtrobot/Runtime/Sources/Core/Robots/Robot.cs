using System;
using System.Collections.Generic;
using Loki;

using UnityEngine;
using System.Diagnostics;
using LitJson;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	//[HierarchyItem(fontColorString = "#FFFF00")]
	[DebuggerDisplay("Name = {name}, AABB = {bounds}")]
	[NameToType]
	public abstract partial class Robot : Group, IRobot
	{
		[Tooltip("用于标记此模型是否主动调用Initialize, 对于Prefab/SceneObject, 一般为true")]
		public bool autoInitialize = false;

		protected override bool initializeOnAwake
		{
			get
			{
				return autoInitialize;
			}
		}

		public bool isKinematicReady
		{
			get
			{
				foreach (var part in parts)
				{
					if (!part.isKinematicReady)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// 管理基于物理的运动关系
		/// </summary>
		private readonly RobotPhysicsSystem mPhysicsSystem = new RobotPhysicsSystem();

		public IRobotPhysicsSystem physicsSystem { get { return mPhysicsSystem; } }

		public override IRobot robot
		{
			get
			{
				return this;
			}
		}

		public new string name { get => base.name; set => base.name = value; }

		public string nameOnUI { get; set; } = "";

		public static int PartSorted(IPart l, IPart r)
		{
			return 0;
		}

		public void AutoInitialize()
		{
			autoInitialize = true;
		}

		public virtual void OnTransformUpdated()
		{

		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			ForceUpdate();
			if (Application.isPlaying)
				RobotManager.GetOrAlloc().AddRobot(this);
		}

		public override void ForceUpdate()
		{
			// all parts in the robot
			parts.Clear();
			transform.GetComponentsInChildren(true, parts);
			// all groups in the robot
			List<Group> groups = new List<Group>();
			transform.GetComponentsInChildren(true, groups);
			foreach (var group in groups)
			{
				group.Initialize();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (RobotManager.Get())
			{
				RobotManager.Get().RemoveRobot(this);
			}
		}

		public List<PartComponent> GetCommandComponents(bool forceUpdate, List<PartComponent> resultCache = null)
		{
			if (resultCache == null)
				resultCache = new List<PartComponent>();

			if (forceUpdate)
			{
				ForceUpdate();
			}

			foreach (Part part in parts)
			{
				if (forceUpdate)
				{
					part.ForceUpdate();
				}
				foreach (var handle in part.commandHandlers)
				{
					if (handle is PartComponent pc)
					{
						resultCache.Add(pc);
					}
				}
			}

			return resultCache;
		}

		public List<IPartIDComponent> GetIDComponents(bool forceUpdate, List<IPartIDComponent> resultCache = null)
		{
			if (resultCache == null)
				resultCache = new List<IPartIDComponent>();

			if (forceUpdate)
			{
				ForceUpdate();
			}
			foreach (Part part in parts)
			{
				if (forceUpdate)
				{
					part.ForceUpdate();
				}
				foreach (var partComponent in part.partsComponents)
				{
					if (partComponent is IPartIDComponent pc)
					{
						resultCache.Add(pc);
					}
				}
			}

			return resultCache;
		}


		public List<PartComponent> GetCommandComponents(List<PartComponent> resultCache = null)
		{
			if (resultCache == null)
				resultCache = new List<PartComponent>();

			foreach (Part part in parts)
			{
				foreach (var handle in part.commandHandlers)
				{
					if (handle is PartComponent pc)
					{
						resultCache.Add(pc);
					}
				}
			}

			return resultCache;
		}

		public List<IPartIDComponent> GetIDComponents(List<IPartIDComponent> resultCache = null)
		{
			if (resultCache == null)
				resultCache = new List<IPartIDComponent>();

			foreach (Part part in parts)
			{
				foreach (var partComponent in part.partsComponents)
				{
					if (partComponent is IPartIDComponent pc)
					{
						resultCache.Add(pc);
					}
				}
			}

			return resultCache;
		}

#if UNITY_EDITOR
		protected override void DoDrawGizmos()
		{
			//Gizmos.color 
			var settings = UbtrobotSettings.GetOrLoad().gizmosSettings;
			//GizmosUtility.DrawBounds(transform, bounds, settings.drawRobotAABB, settings.robotAABBColor);
			var box = GetComponent<BoxCollider>();
			if (box != null)
			{
				GizmosUtility.DrawBoxCollider(box, settings.drawRobotAABB, settings.robotAABBColor);
			}
		}

		[InspectorMethod(aliasName = "提取Command对象")]
		protected void ExtractCommandGroup()
		{
			ForceUpdate();
			var cmdGroup = transform.FindOrAdd("CommandGroup");
			var groupComponent = cmdGroup.gameObject.GetComponent<PartsGroup>() ?? cmdGroup.gameObject.AddComponent<PartsGroup>();
			foreach (Part part in parts)
			{
				if (part != null)
				{
					part.ForceUpdate();
					if (part.commandHandlers.Count > 0)
					{
						if (PrefabUtility.IsPartOfPrefabAsset(part.gameObject))
						{
							PrefabUtility.UnpackPrefabInstance(part.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
						}
						part.transform.SetParent(cmdGroup);
					}
				}
			}

		}
#endif

		public virtual void OnSelected()
		{

		}

		public virtual void OnUnselected()
		{

		}

		public override ICommandResponseAsync Execute(ICommand command)
		{
			var result = base.Execute(command);
			if (result == null && (command is MiscCommands.StopAllDevicesCommand))
			{
				robot.Stop();
				robot.Rewind();
				// temp code
				var result2 = new CommandResponseAsync(ExploreProtocol.CreateResponse(ProtocolCode.Success, command));
				result2.host = command.host;
				result2.context = command.context;
				result = result2;
			}
			return result;
		}

		protected override void OnStop()
		{
			base.OnStop();

			foreach (var part in parts)
			{
				part.Stop();
			}
		}

		[InspectorMethod(aliasName = "CheckCommands")]
		public void CheckCommands()
		{
			var idComponents = GetIDComponents(true);

			if (idComponents == null)
				return;

			Dictionary<DeviceType, HashSet<int>> devices = new Dictionary<DeviceType, HashSet<int>>();
			foreach (var idComponent in idComponents)
			{
				if (idComponent.deviceID == DeviceType.None)
				{
					DebugUtility.LogError(LoggerTags.Project, "The deviceID of id-component is invalid. : Type {0}, DeviceType {1} ID : {2}", idComponent.GetType(), idComponent.deviceID, idComponent.id);
					continue;
				}

				if (!devices.TryGetValue(idComponent.deviceID, out var hash))
				{
					hash = new HashSet<int>();
					devices.Add(idComponent.deviceID, hash);
				}
				if (idComponent.id == 0)
				{
					DebugUtility.LogError(LoggerTags.Project, "The id of id-component is invalid. : Type {0}, DeviceType {1} ID : {2}", idComponent.GetType(), idComponent.deviceID, idComponent.id);
					continue;
				}
				if (!hash.Add(idComponent.id))
				{
					DebugUtility.LogError(LoggerTags.Project, "The id of id-component is repeating. Type {0}, DeviceType {1} ID : {2}", idComponent.GetType(), idComponent.deviceID, idComponent.id);
					continue;
				}
			}

		}
	}

	public abstract partial class Robot
	{
#if UNITY_EDITOR
		[InspectorMethod(aliasName = "选中路径配置项")]
		public void SelectPathSettings()
		{
			UnityEditor.Selection.activeObject = UbtrobotSettings.GetOrLoad().pathSettings;
		}
#endif

		[InspectorMethod(aliasName = "保存JSON")]
		public void SerializeToFile()
		{
			SerializeToFile(false);
		}

		[InspectorMethod(aliasName = "保存Debug-JSON")]
		public void SerializeToDebugFile()
		{
			SerializeToFile(true);
		}

		public void SerializeToFile(bool debug)
		{
#if UNITY_EDITOR
			var labModelExportPath = UbtrobotSettings.GetOrLoad().pathSettings.GetExportFullPath();

			//DebugUtility.Log(LoggerTags.Project, labModelExportPath);
			FileSystem.Get().CheckDirectory(labModelExportPath);
			var serializer = new DefaultRobotSerializer(this);
			JsonWriter jsonWriter = new JsonWriter();
			if (debug)
			{
				jsonWriter.IndentValue = 4;
				jsonWriter.PrettyPrint = true;
			}
			serializer.ToJson().ToJson(jsonWriter);
			var json = jsonWriter.ToString();
			string wp = string.Concat(labModelExportPath, "/", this.name, ".json.txt");
			FileSystem.Get().WriteAllText(wp, json);
			if (debug)
			{
				DebugUtility.Log(LoggerTags.Project, json);
				AssetManager.OpenAssetInEditor(FileSystem.FullPathToAssetPath(wp));
			}
			AssetDatabase.Refresh();
#endif
		}
	}
}
