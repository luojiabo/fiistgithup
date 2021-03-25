using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using System.IO;

namespace Loki
{
	[Logger]
	public static class DebugUtility
	{
		/// <summary>
		/// Filter tags
		/// </summary>
		private static readonly HashSet<string> msFilterTags = new HashSet<string>();
		private static readonly HashSet<string> msFilterIgnoreTags = new HashSet<string>();
		private static readonly Dictionary<LogType, Color> msLogTypeColors = new Dictionary<LogType, Color>()
		{
			{ LogType.Log, Color.white },
			{ LogType.Warning, Color.yellow },
			{ LogType.Error, Color.red },
			{ LogType.Assert, Color.red },
			{ LogType.Exception, Color.red },
		};

		//
		// 摘要:
		//     Reports whether the development console is visible. The development console cannot
		//     be made to appear using:
		public static bool developerConsoleVisible { get { return Debug.developerConsoleVisible; } set { Debug.developerConsoleVisible = value; } }
		//
		// 摘要:
		//     Get default debug logger.
		public static ILogger unityLogger { get { return Debug.unityLogger; } }
		//
		// 摘要:
		//     In the Build Settings dialog there is a check box called "Development Build".
		public static bool isDebugBuild { get { return Debug.isDebugBuild; } }

		public static Color GetLogColor(LogType logType)
		{
			if (msLogTypeColors.TryGetValue(logType, out var color))
			{
				return color;
			}
			return Color.white;
		}


		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="filter"></param>
		public static void Initialize(LogSettings settings)
		{
			msFilterTags.Clear();
			msFilterIgnoreTags.Clear();
			if (settings != null)
			{
				msFilterTags.UnionWith(settings.filterTags);
				msFilterIgnoreTags.UnionWith(settings.filterIgnoreTags);
				unityLogger.filterLogType = settings.filterLogType;
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message, Object context)
		{
			Debug.Assert(condition, message, context);
		}
		//
		// 摘要:
		//     Assert a condition and logs an error message to the Unity console on failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   context:
		//     Object to which the message applies.
		//
		//   message:
		//     String or object to be converted to string representation for display.
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message, Object context)
		{
			Debug.Assert(condition, message, context);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			Debug.Assert(condition, message);
		}

		//
		// 摘要:
		//     Assert a condition and logs an error message to the Unity console on failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   context:
		//     Object to which the message applies.
		//
		//   message:
		//     String or object to be converted to string representation for display.
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			Debug.Assert(condition, message);
		}

		//
		// 摘要:
		//     Assert a condition and logs an error message to the Unity console on failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   context:
		//     Object to which the message applies.
		//
		//   message:
		//     String or object to be converted to string representation for display.
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, Object context)
		{
			Debug.Assert(condition, context);
		}

		//
		// 摘要:
		//     Assert a condition and logs an error message to the Unity console on failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   context:
		//     Object to which the message applies.
		//
		//   message:
		//     String or object to be converted to string representation for display.
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition)
		{
			Debug.Assert(condition);
		}

		//
		// 摘要:
		//     Assert a condition and logs a formatted error message to the Unity console on
		//     failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   format:
		//     A composite format string.
		//
		//   args:
		//     Format arguments.
		//
		//   context:
		//     Object to which the message applies.
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, Object context, string format, params object[] args)
		{
			Debug.AssertFormat(condition, context, format, args);
		}

		//
		// 摘要:
		//     Assert a condition and logs a formatted error message to the Unity console on
		//     failure.
		//
		// 参数:
		//   condition:
		//     Condition you expect to be true.
		//
		//   format:
		//     A composite format string.
		//
		//   args:
		//     Format arguments.
		//
		//   context:
		//     Object to which the message applies.
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			Debug.AssertFormat(condition, format, args);
		}

		//
		// 摘要:
		//     Pauses the editor.
		public static void Break()
		{
			Debug.Break();
		}

		//
		// 摘要:
		//     Clears errors from the developer console.
		public static void ClearDeveloperConsole()
		{
			Debug.ClearDeveloperConsole();
		}

		public static void DebugBreak()
		{
			Debug.DebugBreak();
		}

		//
		// 摘要:
		//     Draws a line between specified start and end points.
		//
		// 参数:
		//   start:
		//     Point in world space where the line should start.
		//
		//   end:
		//     Point in world space where the line should end.
		//
		//   color:
		//     Color of the line.
		//
		//   duration:
		//     How long the line should be visible for.
		//
		//   depthTest:
		//     Should the line be obscured by objects closer to the camera?
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			Debug.DrawLine(start, end, color, duration);
		}

		//
		// 摘要:
		//     Draws a line between specified start and end points.
		//
		// 参数:
		//   start:
		//     Point in world space where the line should start.
		//
		//   end:
		//     Point in world space where the line should end.
		//
		//   color:
		//     Color of the line.
		//
		//   duration:
		//     How long the line should be visible for.
		//
		//   depthTest:
		//     Should the line be obscured by objects closer to the camera?
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			Debug.DrawLine(start, end, color);
		}

		//
		// 摘要:
		//     Draws a line between specified start and end points.
		//
		// 参数:
		//   start:
		//     Point in world space where the line should start.
		//
		//   end:
		//     Point in world space where the line should end.
		//
		//   color:
		//     Color of the line.
		//
		//   duration:
		//     How long the line should be visible for.
		//
		//   depthTest:
		//     Should the line be obscured by objects closer to the camera?
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
		{
			Debug.DrawLine(start, end, color, duration, depthTest);
		}

		//
		// 摘要:
		//     Draws a line between specified start and end points.
		//
		// 参数:
		//   start:
		//     Point in world space where the line should start.
		//
		//   end:
		//     Point in world space where the line should end.
		//
		//   color:
		//     Color of the line.
		//
		//   duration:
		//     How long the line should be visible for.
		//
		//   depthTest:
		//     Should the line be obscured by objects closer to the camera?
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			Debug.DrawLine(start, end);
		}

		//
		// 摘要:
		//     Draws a line from start to start + dir in world coordinates.
		//
		// 参数:
		//   start:
		//     Point in world space where the ray should start.
		//
		//   dir:
		//     Direction and length of the ray.
		//
		//   color:
		//     Color of the drawn line.
		//
		//   duration:
		//     How long the line will be visible for (in seconds).
		//
		//   depthTest:
		//     Should the line be obscured by other objects closer to the camera?
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			Debug.DrawLine(start, dir, color, duration);
		}
		//
		// 摘要:
		//     Draws a line from start to start + dir in world coordinates.
		// 参数:
		//   start:
		//     Point in world space where the ray should start.
		//
		//   dir:
		//     Direction and length of the ray.
		//
		//   color:
		//     Color of the drawn line.
		//
		//   duration:
		//     How long the line will be visible for (in seconds).
		//
		//   depthTest:
		//     Should the line be obscured by other objects closer to the camera?
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
		{
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}


		//
		// 摘要:
		//     Draws a line from start to start + dir in world coordinates.
		//
		// 参数:
		//   start:
		//     Point in world space where the ray should start.
		//
		//   dir:
		//     Direction and length of the ray.
		//
		//   color:
		//     Color of the drawn line.
		//
		//   duration:
		//     How long the line will be visible for (in seconds).
		//
		//   depthTest:
		//     Should the line be obscured by other objects closer to the camera?
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			Debug.DrawRay(start, dir);
		}

		//
		// 摘要:
		//     Draws a line from start to start + dir in world coordinates.
		//
		// 参数:
		//   start:
		//     Point in world space where the ray should start.
		//
		//   dir:
		//     Direction and length of the ray.
		//
		//   color:
		//     Color of the drawn line.
		//
		//   duration:
		//     How long the line will be visible for (in seconds).
		//
		//   depthTest:
		//     Should the line be obscured by other objects closer to the camera?
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			Debug.DrawRay(start, dir, color);
		}

		public static void Log(LogType type, string tag, string format, params object[] args)
		{
			Log(type, null, tag, format, args);
		}

		public static void Trace(LogType type, string tag, string format, params object[] args)
		{
			Trace(type, null, tag, format, args);
		}

		private static void UnityLogFormat(LogType type, LogOption option, Object context, string tag, string format, params object[] args)
		{
			try
			{
				if (args.Length > 0)
				{
					Debug.LogFormat(type, option, context, string.Concat(tag, ":", format), args);
				}
				else
				{
					if (type == LogType.Log)
					{
						Debug.Log(string.Concat(tag, ":", format), context);
					}
					else if (type == LogType.Warning)
					{
						Debug.LogWarning(string.Concat(tag, ":", format), context);
					}
					else if (type == LogType.Error)
					{
						Debug.LogError(string.Concat(tag, ":", format), context);
					}
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "The formatting-string or argument is abnormal.");
				DebugUtility.LogException(ex);
			}
		}

		private static void UnityLogFormat(LogType type, Object context, string tag, string format, params object[] args)
		{
			try
			{
				switch (type)
				{
					case LogType.Log:
						{
							Debug.LogFormat(context, string.Concat(tag, ":", format), args);
							break;
						}
					case LogType.Warning:
						{
							Debug.LogWarningFormat(context, string.Concat(tag, ":", format), args);
							break;
						}
					case LogType.Error:
						{
							Debug.LogErrorFormat(context, string.Concat(tag, ":", format), args);
							break;
						}
					case LogType.Assert:
						{
							Debug.AssertFormat(context, string.Concat(tag, ":", format), args);
							break;
						}
					default:
						{
							break;
						}
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Engine, "The formatting-string or argument is abnormal.");
				DebugUtility.LogException(ex);
			}
		}

		private static string GenerateSimpleTrace(int skipframe, string tag, string format, params object[] args)
		{
			//Loki.DebugUtility:Trace(LogType, Object, String, String, Object[]) (at Assets/Loki/Runtime/Engine/DebugUtility.cs:451)
			var st = new StackTrace(skipframe, true);
			if (st.FrameCount > 2)
			{
				if (args.Length > 0)
				{
					return string.Concat(tag, " : ", string.Format(format, args),
						"\n",
						st.GetFrame(2).GetStackFrameInfo());
				}
				else
				{
					return string.Concat(tag, " : ", format,
						"\n",
						st.GetFrame(2).GetStackFrameInfo());
				}
			}
			if (args.Length > 0)
				return string.Concat(tag, " : ", string.Format(format, args));
			else
				return string.Concat(tag, " : ", format);
		}

		public static void Log(LogType type, Object context, string tag, string format, params object[] args)
		{
			if (FilterLog(type, tag))
			{
				//Unity2018LogFormat(type, context, tag, format, args);
#if UNITY_2019
				UnityLogFormat(type, LogOption.NoStacktrace, context, tag, format, args);
				// Debug.LogFormat(type, LogOption.NoStacktrace, context, GenerateSimpleTrace(2, tag, format, args));
#else
				UnityLogFormat(type, context, tag, format, args);
#endif
			}
		}

		public static void Trace(LogType type, Object context, string tag, string format, params object[] args)
		{
			if (FilterLog(type, tag))
			{
				//Unity2018LogFormat(type, context, tag, format, args);
#if UNITY_2019
				UnityLogFormat(type, LogOption.None, context, tag, format, args);
				// Debug.LogFormat(type, LogOption.None, context, string.Concat(tag, " : ", format), args);
#else
				UnityLogFormat(type, context, tag, format, args);
#endif

			}
		}

		public static void Log(string tag, string format, params object[] args)
		{
			Log(LogType.Log, tag, format, args);
		}

		public static void LogWarning(string tag, string format, params object[] args)
		{
			Log(LogType.Warning, tag, format, args);
		}

		public static void LogError(string tag, string format, params object[] args)
		{
			Log(LogType.Error, tag, format, args);
		}

		public static void LogTrace(string tag, string format, params object[] args)
		{
			Trace(LogType.Log, tag, format, args);
		}

		public static void LogWarningTrace(string tag, string format, params object[] args)
		{
			Trace(LogType.Warning, tag, format, args);
		}

		public static void LogErrorTrace(string tag, string format, params object[] args)
		{
			Trace(LogType.Error, tag, format, args);
		}

		public static void LogException(Exception ex, Object context)
		{
			Debug.LogException(ex, context);
		}

		public static void LogException(Exception ex)
		{
			Debug.LogException(ex);
		}

		public static bool FilterLog(LogType logType, string tag)
		{
			if (unityLogger == null)
				return false;
			if (logType == LogType.Exception)
				return true;

			return unityLogger.filterLogType >= logType && !msFilterIgnoreTags.Contains(tag) && (msFilterTags.Count == 0 || msFilterTags.Contains(tag));
		}

		public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Debug.DrawRay(pos, direction);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength);
			Debug.DrawRay(pos + direction, left * arrowHeadLength);
		}

		public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Debug.DrawRay(pos, direction, color);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
		
		public static string TrimRedundancyInfo(string stackTrace)
		{
			if (string.IsNullOrEmpty(stackTrace))
				return string.Empty;

			var blacklist = new List<string>
			{
				typeof(DebugUtility).FullName,
				typeof(UnityEngine.Debug).FullName,
			};

			var reader = new StringReader(stackTrace);
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				bool foundBlackName = line.ContainsAny(blacklist);
				if (!foundBlackName)
				{
					break;
				}
			}
			line = line ?? string.Empty;
			return string.Concat(line, "\n", reader.ReadToEnd());
		}
	}
}
