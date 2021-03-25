using System;
using Loki;

namespace Ubtrobot
{
	public static class ModelFBXParser
	{
		public static readonly string GroupMark = "Group";

		public static PartUnit ParseUnit(this string name, out string customValue)
		{
			var source = string.Empty;
			customValue = string.Empty;
			var index = name.LastIndexOf('_');
			if (index < 0)
			{
				source = name;
			}
			else
			{
				//获取"_001"这类自定义字段
				var custom = name.Substring(index + 1);
				try
				{
					Convert.ToInt32(custom);
					source = name.Substring(0, index);
					customValue = custom;
				}
				catch (Exception)
				{
					source = name;
				}
			}

			PartUnit part = new PartUnit();
			//if (!PartLibraryManager.Instance.TryGetPartUnit(source, out part))
			//{
			//	DebugUtility.LogError(LoggerTags.Project, "命名出错！{0}", name);
			//}
			return part;
		}

		public static string ToLegalName(string name)
		{
			var index = name.IndexOf(' ');
			if (index > 0) name = name.Substring(0, index);
			return name;
		}
	}
}
