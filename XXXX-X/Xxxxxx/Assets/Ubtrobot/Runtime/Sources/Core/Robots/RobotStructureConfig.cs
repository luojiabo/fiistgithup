using System;
using System.Collections.Generic;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public enum PartType
	{
		Normal,
		Bone,
		MC,
		Wire,
		Servo,
		Sensor,
		Motor,
	}
	public struct DTransform
	{
		public Vector3 position;
		public Vector3 angle;
		public Vector3 scale;

		public static string VectorToString(Vector3 vector3)
		{
			string ret = string.Concat(vector3.x, ",", vector3.y, ",", vector3.z);
			return ret;
		}

		public static Vector3 StringToVector(string vectorString)
		{
			if (string.IsNullOrEmpty(vectorString)) return Vector3.zero;
			string[] values = vectorString.Split(',');
			if (values.Length != 3) return Vector3.zero;

			return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
		}

		public static DTransform ParseTransform(JsonData transJson)
		{
			var transform = new DTransform();
			if (transJson == null)
			{
				transform.position = Vector3.zero;
				transform.angle = Vector3.zero;
				transform.scale = Vector3.one;
				return transform;
			}

			transform.position = StringToVector((string)transJson["Position"]);
			transform.angle = StringToVector((string)transJson["Angle"]);
			transform.scale = StringToVector((string)transJson["Scale"]);

			return transform;
		}
	}

	public class PartData
	{
		public virtual PartType partType => PartType.Normal;
		public string name;//零件名称
		public string source;//零件资源名称
		public string parent;//父节点名称
		public DTransform transform;//变换信息

		public virtual JsonData Serialize()
		{
			var partJson = new JsonData();
			partJson["Type"] = partType.ToString();
			partJson["Name"] = name;
			partJson["Source"] = source;
			partJson["Parent"] = parent;
			partJson["Position"] = DTransform.VectorToString(transform.position);
			partJson["Angle"] = DTransform.VectorToString(transform.angle);
			partJson["Scale"] = DTransform.VectorToString(transform.scale);
			return partJson;
		}

		public virtual void Deserialize(JsonData json)
		{
			name = (string)json["Name"];
			source = (string)json["Source"];
			parent = (string)json["Parent"];

			transform = new DTransform();
			var pos = (string)json["Position"];
			transform.position = DTransform.StringToVector(pos);
			var angle = (string)json["Angle"];
			transform.angle = DTransform.StringToVector(angle);
			var scale = (string)json["Scale"];
			transform.scale = DTransform.StringToVector(scale);
		}
	}

	public class BoneData : PartData
	{
		public override PartType partType => PartType.Bone;
		public readonly List<string> items = new List<string>();
		public string num;//编号

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;

			var itemsJson = new JsonData();
			child["Num"] = num;
			child["Items"] = itemsJson;
			for (int i = 0; i < items.Count; i++)
			{
				itemsJson.Add(items[i]);
			}
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			num = (string)append["Num"];
			var itemsJson = append["Items"];
			for (int i = 0; i < itemsJson.Count; i++)
			{
				items.Add((string)itemsJson[i]);
			}
		}
	}

	public class MCData : PartData
	{
		public override PartType partType => PartType.MC;
		public int type;//类型

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;
			child["Type"] = type;
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			type = (int)append["Type"];
		}
	}

	public class WireData : PartData
	{
		public override PartType partType => PartType.Wire;
		public string num;//编号

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;
			child["Num"] = num;
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			num = (string)append["Num"];
		}
	}

	public class ServoData : PartData
	{
		public override PartType partType => PartType.Servo;
		public int type;//0：舵机旋转；1：舵盘旋转 
		public int id;//Id
		public int angle;//初始旋转角度

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;
			child["Type"] = type;
			child["Id"] = id;
			child["Angle"] = angle;
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			type = (int)append["Type"];
			id = (int)append["Id"];
			angle = (int)append["Angle"];
		}
	}

	public class SensorData : PartData
	{
		public override PartType partType => PartType.Sensor;
		public int type;//超声，亮度等等
		public int id;//Id

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;
			child["Type"] = type;
			child["Id"] = id;
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			type = (int)append["Type"];
			id = (int)append["Id"];
		}
	}

	public class MotorData : PartData
	{
		public override PartType partType => PartType.Motor;
		public int id;//Id

		public override JsonData Serialize()
		{
			var partJson = base.Serialize();
			var child = new JsonData();
			partJson[partType.ToString()] = child;
			child["Id"] = id;
			return partJson;
		}

		public override void Deserialize(JsonData json)
		{
			base.Deserialize(json);
			var append = json[partType.ToString()];
			id = (int)append["Id"];
		}
	}

	/// <summary>
	/// 模型构成配置
	/// </summary>
	public class RobotStructureConfig : ISerializer
	{
		public int serializedVersion => 1;

		public string modelId { get; private set; }
		public DTransform buildTransform;
		public DTransform motionTransform;
		public readonly Dictionary<string, List<PartData>> parentToChildren = new Dictionary<string, List<PartData>>();
		public readonly List<PartData> partsList = new List<PartData>();

		public RobotStructureConfig(string modelId)
		{
			this.modelId = modelId;
		}

		#region 序列化
		public string Serialize()
		{
			var json = new JsonData();
			ToTransform("BuildTransform", json, buildTransform);
			ToTransform("MotionTransform", json, motionTransform);
			var partsJson = new JsonData();
			json["Parts"] = partsJson;
			for (int i = 0; i < partsList.Count; i++)
			{
				var part = partsList[i];
				var partJson = part.Serialize();
				partsJson.Add(partJson);
			}
			return json.ToJson();
		}

		private void ToTransform(string tagName, JsonData root, DTransform dTransform)
		{
			var transJson = new JsonData();
			root[tagName] = transJson;
			transJson["Position"] = DTransform.VectorToString(dTransform.position);
			transJson["Angle"] = DTransform.VectorToString(dTransform.angle);
			transJson["Scale"] = DTransform.VectorToString(dTransform.scale);
		}
		#endregion

		#region 反序列化
		public bool Deserialize(JsonData json)
		{
			try
			{
				buildTransform = DTransform.ParseTransform(json["BuildTransform"]);
				motionTransform = DTransform.ParseTransform(json["MotionTransform"]);
				ParsePartDatas(json["Parts"]);

				return true;
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
				return false;
			}
		}

		private void ParsePartDatas(JsonData partsJson)
		{
			for (int i = 0; i < partsJson.Count; i++)
			{
				var partJson = partsJson[i];
				var data = ParsePartData(partJson);
				AddPart(data);
			}
		}

		private void AddPart<T>(T t) where T : PartData
		{
			if (t == null) return;
			partsList.Add(t);

			var parentName = t.parent;
			if (string.IsNullOrEmpty(parentName)) return;
			List<PartData> children;
			if (!parentToChildren.TryGetValue(parentName, out children))
			{
				children = new List<PartData>();
				parentToChildren[parentName] = children;
			}
			children.Add(t);
		}
		#endregion

		#region 静态方法
		private static PartData ParsePartData(JsonData partJson)
		{
			PartData data;
			PartType type;
			var typeString = (string)partJson["Type"];
			type = typeString.ToEnum(PartType.Normal);
			switch (type)
			{
				case PartType.Normal:
					data = new PartData();
					break;
				case PartType.Bone:
					data = new BoneData();
					break;
				case PartType.MC:
					data = new MCData();
					break;
				case PartType.Wire:
					data = new WireData();
					break;
				case PartType.Servo:
					data = new ServoData();
					break;
				case PartType.Sensor:
					data = new SensorData();
					break;
				case PartType.Motor:
					data = new MotorData();
					break;
				default:
					data = new PartData();
					break;
			}

			data.Deserialize(partJson);
			return data;
		}
		#endregion

	}
}
