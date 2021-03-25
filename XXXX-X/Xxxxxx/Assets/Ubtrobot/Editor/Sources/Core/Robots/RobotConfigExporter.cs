using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ubtrobot;

namespace Loki
{
	public class RobotConfigExporter
	{
		private readonly Dictionary<int, string> m_ToGroupName = new Dictionary<int, string>();

		private string GetStructurePath(string directory, string fileName)
		{
			return PathUtility.Combine(directory, fileName);
		}

		public static void Export(string directory)
		{
			var exporter = new RobotConfigExporter();
			exporter.ExportConfig(directory);
		}

		private void ExportConfig(string directory)
		{
			var motion = UnityEditor.Selection.activeGameObject.transform;
			motion.position = Vector3.zero;
			motion.localScale = Vector3.one;
			var children = GetModelChildren(motion);
			if (children.Count <= 0) return;

			var datas = GetData(children);
			ExportStructure(GetStructurePath(directory, string.Concat(motion.name, ".json")), datas);
			AssetDatabase.Refresh();
		}

		private void ExportStructure(string path, List<PartData> datas)
		{
			var config = new RobotStructureConfig("");
			var standard = new DTransform();
			standard.scale = Vector3.one;
			config.buildTransform = standard;
			config.motionTransform = standard;
			config.partsList.AddRange(datas);
			FileSystem.Get().WriteAllText(path, config.Serialize());
		}

		private List<Transform> GetModelChildren(Transform motion)
		{
			List<Transform> list = new List<Transform>();
			var anim = motion.GetComponent<Animation>();
			if (anim == null)
			{
				Debug.LogError("当前对象非模型: " + motion.name);
				return list;
			}
			var count = motion.childCount;
			for (int i = 0; i < count; i++)
			{
				var child = motion.GetChild(i);
				if (!child.gameObject.activeSelf)
				{
					Debug.LogError(string.Format("当前模型有隐藏节点: {0}", child.name));
					break;
				}
				if (child.name.StartsWith(ModelFBXParser.GroupMark))
				{
					list.Add(child);
					list.AddRange(GetGroupChildren(child));
				}
				else
				{
					list.Add(child);
				}
			}
			return list;
		}

		private List<Transform> GetGroupChildren(Transform group)
		{
			List<Transform> list = new List<Transform>();
			var count = group.childCount;
			for (int i = 0; i < count; i++)
			{
				var child = group.GetChild(i);
				if (!child.gameObject.activeSelf)
				{
					Debug.LogErrorFormat("当前模型有隐藏节点: {0}", child.name);
					break;
				}
				list.Add(child);
				m_ToGroupName[child.GetInstanceID()] = group.name;
			}
			return list;
		}

		private List<PartData> GetData(List<Transform> children)
		{
			var list = new List<PartData>();
			for (int i = 0; i < children.Count; i++)
			{
				var child = children[i];
				string customValue;
				var partUnit = ModelFBXParser.ParseUnit(child.name, out customValue);
				var data = ParsePartData(child, partUnit, customValue);

				list.Add(data);
			}
			return list;
		}

		private string GetParentName(Transform child)
		{
			string name;
			if (m_ToGroupName.TryGetValue(child.GetInstanceID(), out name))
			{
				return name;
			}
			return string.Empty;
		}

		private PartData ParsePartData(Transform trans, PartUnit partUnit, string customValue)
		{
			PartData partData;
			switch (partUnit.type)
			{
				case PartType.Bone:
					{
						var data = new BoneData();
						for (int i = 0; i < trans.childCount; i++)
						{
							var t = trans.GetChild(i);
							if (!t.name.StartsWith(ModelFBXParser.GroupMark)) continue;

							var group = t;
							for (int j = 0; j < @group.childCount; j++)
							{
								var child = @group.GetChild(j);
								data.num = customValue;
								var childName = ModelFBXParser.ToLegalName(child.name);
								data.items.Add(childName);
							}
							break;
						}
						partData = data;
						break;
					}
				case PartType.MC:
					{
						var data = new MCData();
						data.type = 1;
						partData = data;
						break;
					}
				case PartType.Wire:
					{
						var data = new WireData();
						data.num = customValue;
						partData = data;
						break;
					}
				case PartType.Servo:
					{
						var data = new ServoData();
						data.id = int.Parse(customValue);
						data.type = 1;
						data.angle = 120;
						partData = data;
						break;
					}
				case PartType.Motor:
					{
						var data = new MotorData();
						data.id = int.Parse(customValue);
						partData = data;
						break;
					}
				case PartType.Sensor:
					{
						var data = new SensorData();
						data.id = int.Parse(customValue);
						partData = data;
						break;
					}
				default:
					{
						partData = new PartData();
						break;
					}
			}

			partData.name = trans.name;
			partData.source = partUnit.source;
			partData.transform = new DTransform();
			partData.transform.position = trans.localPosition;
			partData.transform.angle = trans.localEulerAngles;
			partData.transform.scale = GetScale(trans, partData.partType);
			partData.parent = GetParentName(trans);

			return partData;
		}

		private static Vector3 GetScale(Transform motion, PartType type)
		{
			if (type == PartType.Wire)
			{
				return Vector3.zero;
			}
			return motion.localScale;
		}
	}
}
