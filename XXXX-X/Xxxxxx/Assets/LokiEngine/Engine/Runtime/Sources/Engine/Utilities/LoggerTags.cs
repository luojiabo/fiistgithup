using System;
using System.Collections;
using System.Collections.Generic;

namespace Loki
{
	[LoggerTags]
	public static class LoggerTags
	{
		private static readonly List<string> msAllLoggerTags = new List<string>();

		public static readonly string NoTag = "NoTag";
		public static readonly string Engine = "Engine";
		public static readonly string TypeCheck = "TypeCheck";
		public static readonly string AssetManager = "AssetManager";
		public static readonly string BuildSystem = "BuildSystem";
		public static readonly string Console = "Console";
		public static readonly string Online = "Online";
		public static readonly string Module = "Module";
		public static readonly string ObjectPool = "ObjectPool";
		public static readonly string TaskQueue = "TaskQueue";
		public static readonly string GlobalObject = "GlobalObject";
		public static readonly string InputSystem = "InputSystem";
		public static readonly string CameraSystem = "CameraSystem";
		public static readonly string UI = "UI";

		public static string Project { get { return UnityEngine.Application.productName; } }

		public static List<string> CollectTags()
		{
			if (msAllLoggerTags.Count > 0)
				return msAllLoggerTags;

			var types = GlobalReflectionCache.FindTypes<LoggerTagsAttribute>(true);
			if (types != null)
			{
				foreach (var tagType in types)
				{
					var allFields = tagType.GetFields();
					foreach (var fieldInfo in allFields)
					{
						if (fieldInfo.IsStatic && fieldInfo.FieldType == typeof(string))
						{
							string o = fieldInfo.GetValue(null) as string;
							if (!string.IsNullOrEmpty(o))
							{
								msAllLoggerTags.Union(o);
							}
						}
					}
				}
			}

			// DebugUtility.LogTrace(LoggerTags.Engine, "The Logger tags count is {0}", msAllLoggerTags.Count);
			return msAllLoggerTags;
		}
	}
}
