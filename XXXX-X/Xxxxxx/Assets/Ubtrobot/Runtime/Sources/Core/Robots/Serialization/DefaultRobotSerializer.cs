using System;
using System.Collections.Generic;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class DefaultRobotSerializer : RobotSerializer<IRobot>
	{
		private static readonly List<Component> msComponentCache = new List<Component>();
		private readonly List<Part> mParts = new List<Part>();
		private readonly List<PartsGroup> mPartsGroup = new List<PartsGroup>();

		private static int msUnknownID = -1;

		public static bool enableDebugInfo { get; set; } = false;

		public DefaultRobotSerializer()
		{

		}

		public DefaultRobotSerializer(IRobot robot)
		: base(robot)
		{
		}

		private static void RevertUnknownID()
		{
			msUnknownID = -1;
		}

		private int IndexOfPartsGroup(PartsGroup g)
		{
			var idx = mPartsGroup.FindIndex(e => g == e);
			return idx >= 0 ? idx : msUnknownID--;
		}

		private int IndexOfPart(Part p)
		{
			var idx = mParts.FindIndex(e => p == e);
			return idx >= 0 ? idx : msUnknownID--;
		}

		protected override void SerializeToJson(LitJson.JsonData modelData)
		{
			RevertUnknownID();

			mPartsGroup.Clear();
			mParts.Clear();
			mRobot.transform.GetComponentsInChildren(true, mPartsGroup);
			mRobot.transform.GetComponentsInChildren(true, mParts);

			GenerateUniquePath(mRobot.transform);
			SerializeToJson(modelData, mRobot.transform);

			var assets = new Dictionary<string, int>();
			PredicatePreloadingAssets(modelData, assets);
			if (assets.Count > 0)
			{
				modelData[SerializationConst.assets] = LitJsonHelper.CreateObjectJsonData(assets);
			}
		}

		protected override RobotDataModel DeserializeFromJson(LitJson.JsonData modelData)
		{
			RobotDataModel model = new RobotDataModel();
			model.Parse(modelData);
			return model;
		}

		private void GenerateUniquePath(Transform transform)
		{
			bool isPart = false;
			if (transform.TryGetComponent<Part>(out var part))
			{
				isPart = true;

				if (part.GetComponentInChildren(typeof(AdvancedPartComponent), true))
				{
					// 旧自动化ID风格
					var idxOf_ = part.name.LastIndexOf('_');
					if (idxOf_ >= 0 && idxOf_ < part.name.Length - 1)
					{
						string idPart = part.name.Substring(idxOf_ + 1);
						if (int.TryParse(idPart, out var lastID))
						{
							part.name = part.name.Substring(0, idxOf_);
						}
					}
				}

				var idx = IndexOfPart(part);
				part.name = part.ExportName(idx);
			}
			else if (transform.TryGetComponent<PartsGroup>(out var group))
			{
				var idx = IndexOfPartsGroup(group);
				group.name = group.ExportName(idx);
			}

			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i);
				// 如果是零件， 则只关心@开头的部分, 如果没打上EditorOnly Tag也属于不需要序列化部分
				if (isPart && (!child.name.StartsWith(SerializationConst.programFlag) || !child.CompareTag(TagUtility.EditorOnly)))
				{
					continue;
				}
				GenerateUniquePath(child);
			}
		}

		private void SerializeToJson(LitJson.JsonData node, Transform transform)
		{
			ComponentSerializer.ToJson(transform, node);

			bool isPart = false;
			if (transform.TryGetComponent<Part>(out var part))
			{
				isPart = true;
			}

			msComponentCache.Clear();
			transform.GetComponents(typeof(Component), msComponentCache);
			var components = msComponentCache;

			var comsJson = new JsonData();
			foreach (var com in components)
			{
				var comJson = ComponentSerializer.ComponentToJson(com);
				if (comJson != null && comJson.GetJsonType() != JsonType.None)
				{
					comsJson.Add(comJson);
				}
			}
			msComponentCache.Clear();

			int childCount = transform.childCount;
			var childrenJson = new JsonData();
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i);
				// 如果是零件， 则只关心@开头的部分, 如果没打上EditorOnly Tag也属于不需要序列化部分
				if (isPart && (!child.name.StartsWith(SerializationConst.programFlag) || !child.CompareTag(TagUtility.EditorOnly)))
				{
					continue;
				}

				var childJson = new JsonData();
				SerializeToJson(childJson, child);
				if (childJson.GetJsonType() != JsonType.None)
					childrenJson.Add(childJson);
			}

			if (comsJson.GetJsonType() != JsonType.None)
				node[SerializationConst.components] = comsJson;

			if (childrenJson.GetJsonType() != JsonType.None)
				node[SerializationConst.children] = childrenJson;
		}

		private void PredicatePreloadingAssets(LitJson.JsonData jsonNode, Dictionary<string, int> preload)
		{
			if (jsonNode == null || jsonNode.GetJsonType() == JsonType.None)
			{
				return;
			}

			if (jsonNode.ContainsKey(SerializationConst.components))
			{
				var componentsJson = jsonNode[SerializationConst.components];
				int componentsCount = componentsJson.Count;
				for (int i = 0; i < componentsCount; i++)
				{
					var componentJson = componentsJson[i];
					if ((string)componentJson[SerializationConst.type] == typeof(Part).Name)
					{
						if (!componentJson.ContainsKey(SerializationConst.userDatas))
						{
							throw new Exception(componentJson.ToJson());
						}
						if (!componentJson[SerializationConst.userDatas].ContainsKey(SerializationConst.address))
						{
							throw new Exception(componentJson.ToJson());
						}
						string partAddress = (string)componentJson[SerializationConst.userDatas][SerializationConst.address];
						preload[partAddress] = preload.FindOrAdd(partAddress) + 1;
					}
				}
			}

			if (jsonNode.ContainsKey(SerializationConst.children))
			{
				var childrenJson = jsonNode[SerializationConst.children];
				int childrenCount = childrenJson.Count;
				for (int i = 0; i < childrenCount; i++)
				{
					var childJson = childrenJson[i];
					PredicatePreloadingAssets(childJson, preload);
				}
			}
		}

	}
}
