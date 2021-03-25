using System;
using System.Collections.Generic;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	class RobotComponentDataModel
	{
		private JsonData mUserData;

		public string type;
		public bool alloc;

		public JsonData userData => mUserData;

		public Component component;

		public RobotComponentDataModel(JsonData componentJson)
		{
			ComponentDeserializer.ComponentDataFromJson(componentJson, out type, out alloc, out mUserData);
		}
	}

	class RobotGameObjectDataModel
	{
		public string name;
		public string path;
		public int layer;
		public string tag;
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;

		public readonly List<RobotComponentDataModel> components;
		public readonly List<RobotGameObjectDataModel> children;
		public Transform transform;

		public bool IsAsset(out string address)
		{
			foreach (var component in components)
			{
				if (component.type == typeof(Part).Name && component.userData.ContainsKey(SerializationConst.address))
				{
					address = (string)component.userData[SerializationConst.address];
					return true;
				}
			}
			address = string.Empty;
			return false;
		}

		public RobotGameObjectDataModel(JsonData jsonNode)
		{
			name = "";
			path = "";
			tag = "";
			layer = LayerUtility.DefaultLayer;
			localScale = Vector3.zero;
			localPosition = Vector3.zero;
			localRotation = Quaternion.identity;
			localScale = Vector3.zero;
			components = new List<RobotComponentDataModel>();
			children = new List<RobotGameObjectDataModel>();

			Create(jsonNode);
		}

		private void Create(JsonData jsonNode)
		{
			ComponentDeserializer.FromJson(jsonNode, out name, out path, out layer, out tag, out localPosition, out localRotation, out localScale);
			if (Application.isPlaying)
			{
				tag = tag == TagUtility.EditorOnly ? TagUtility.Untagged : tag;
			}

			ComponentDeserializer.ComponentsFromJson(jsonNode, (comJson) =>
			{
				components.Add(new RobotComponentDataModel(comJson));
			});

			ComponentDeserializer.ChildrenFromJson(jsonNode, (childJson) =>
			{
				children.Add(new RobotGameObjectDataModel(childJson));
			});
		}
	}

	public class RobotDataModel
	{
		private LitJson.JsonData mModelData = null;
		private RobotGameObjectDataModel mGameObjectDataModel;

		private readonly Dictionary<string, int> mAssetDatas = new Dictionary<string, int>();

		public Dictionary<string, int> assetDatas => mAssetDatas;

		public RobotDataModel()
		{
		}

		public bool Parse(LitJson.JsonData modelData)
		{
			mAssetDatas.Clear();

			mModelData = modelData;
			if (modelData.ContainsKey(SerializationConst.assets))
			{
				LitJsonHelper.ParseJsonObject(modelData[SerializationConst.assets], assetDatas);
			}
			mGameObjectDataModel = new RobotGameObjectDataModel(modelData);
			return true;
		}

		public Transform Rebuild(Transform parent, Dictionary<string, Queue<GameObject>> parts)
		{
			var tr = RebuildTransform(parent, mGameObjectDataModel, parts);
			RebuildComponents(mGameObjectDataModel);
			return tr;
		}

		private Transform RebuildTransform(Transform parent, RobotGameObjectDataModel dataModel, Dictionary<string, Queue<GameObject>> partsGroup)
		{
			GameObject node = null;
			if (dataModel.IsAsset(out var address))
			{
				if (partsGroup.TryGetValue(address, out var parts))
				{
					if (parts.Count > 0)
					{
						node = parts.Dequeue();
						node.name = dataModel.name;
					}
				}
			}
			if (node == null)
			{
				node = new GameObject(dataModel.name);
			}
			node.transform.SetParent(parent, false);
			node.transform.localPosition = dataModel.localPosition;
			node.transform.localRotation = dataModel.localRotation;
			node.transform.localScale = dataModel.localScale;
			node.gameObject.layer = dataModel.layer;
			node.gameObject.tag = dataModel.tag;

			dataModel.transform = node.transform;

			foreach (var child in dataModel.children)
			{
				RebuildTransform(node.transform, child, partsGroup);
			}

			return node.transform;
		}

		private void AttachComponents(RobotGameObjectDataModel dataModel)
		{
			foreach (var component in dataModel.components)
			{
				ComponentDeserializer.AttachComponent(dataModel.transform, component);
			}

			foreach (var child in dataModel.children)
			{
				AttachComponents(child);
			}
		}

		private void RebuildComponents(RobotGameObjectDataModel dataModel)
		{
			AttachComponents(dataModel);
			RebuildPathLink(dataModel);
		}

		private void RebuildPathLink(RobotGameObjectDataModel dataModel)
		{
			foreach (var component in dataModel.components)
			{
				ComponentDeserializer.DeserializeFromJson(component);
			}

			foreach (var child in dataModel.children)
			{
				RebuildPathLink(child);
			}
		}
	}
}
