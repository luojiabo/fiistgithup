using System;
using System.Collections.Generic;
using LitJson;
using Loki;

namespace Ubtrobot
{
	public struct PartUnit
	{
		public string source;
		public string id;
		public PartType type;

		public string showId => id;

		public bool IsPeripherals()
		{
			return type == PartType.Servo || type == PartType.Motor || type == PartType.Sensor;
		}
	}
	public class PartLibraryManifest : ISerializer
	{
		public int serializedVersion => 1;

#if UNITY_EDITOR
		private readonly List<string> unitIcons = new List<string>();
		public List<string> GetPartIcons()
		{
			return unitIcons;
		}
#endif

		#region 兼容零件
		private static readonly PartUnit msLP_LTBU = new PartUnit() { source = "LP_LTBU", type = PartType.Normal };
		#endregion

		#region 程序定制零件
		private static readonly PartUnit msGroup = new PartUnit() { source = "Group", type = PartType.Normal };
		private static readonly PartUnit msBone_M87_BLK = new PartUnit() { source = "Bone_M87_BLK", type = PartType.Bone };
		#endregion

		private readonly Dictionary<string, PartUnit> m_KeyToUnit = new Dictionary<string, PartUnit>();



		#region 反序列化
		public bool Deserialize(JsonData json)
		{
			try
			{
				m_KeyToUnit[msLP_LTBU.source] = msLP_LTBU;
				m_KeyToUnit[msGroup.source] = msGroup;
				m_KeyToUnit[msBone_M87_BLK.source] = msBone_M87_BLK;

				var content = json;
				for (int i = 0; i < content.Count; i++)
				{
					var item = content[i];
					var unit = new PartUnit();
					var source = (string)item["Source"];
					unit.source = source;
					unit.id = (string)item["Id"];
					unit.type = ((string)item["Type"]).ToEnum(PartType.Normal);
					m_KeyToUnit[source] = unit;
#if UNITY_EDITOR
					unitIcons.Add(source);
#endif
				}
				return true;
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
				return false;
			}
		}
		#endregion
		public List<string> GetPartPrefabs()
		{
			return new List<string>(m_KeyToUnit.Keys);
		}

		public bool TryGetPartUnit(string key, out PartUnit unit)
		{
			if (m_KeyToUnit.TryGetValue(key, out unit))
			{
				return true;
			}
			return false;
		}

	}
}
