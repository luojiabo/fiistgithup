using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Loki
{
	public sealed class ConsoleEditor : AutoRegisterAssetEditor<ConsoleEditor>
	{
		private static readonly List<string> msBlacklist = new List<string>();
		private static readonly List<string> msFilterStrings = new List<string>();

		private bool isOpenningFile { get; set; }

		public ConsoleEditor()
		{
			isOpenningFile = false;
		}

		/// <summary>
		/// Do not declare InitializeOnLoadMethodAttribute On Generic 
		/// </summary>
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			AutoCollectDebugFilters();
			InitializeOnLoad();
		}

		private static void AutoCollectDebugFilters()
		{
			var loggerTypes = GlobalReflectionCache.FindTypes<LoggerAttribute>(true);
			if (loggerTypes != null && loggerTypes.Count > 0)
			{
				foreach (var type in loggerTypes)
				{
					string typeFileName = string.Concat(type.Name, ".cs");
					msBlacklist.Union(typeFileName);
					msFilterStrings.Union(typeFileName);

					var loggerAttr = type.GetCustomAttribute<LoggerAttribute>(true);
					msBlacklist.Union(loggerAttr.loggerBlacklist);
				}
			}
		}

		private static string GetConsoleStackTrace()
		{
			var consoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
			var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var consoleInstance = fieldInfo.GetValue(null);
			if (consoleInstance != null)
			{
				if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
				{
					var ListViewStateType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
					fieldInfo = consoleWindowType.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
					var listView = fieldInfo.GetValue(consoleInstance);
					fieldInfo = ListViewStateType.GetField("row", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
					int row = (int)fieldInfo.GetValue(listView);
					fieldInfo = consoleWindowType.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
					string activeText = fieldInfo.GetValue(consoleInstance).ToString();
					return activeText;
				}
			}
			return null;
		}

		private static bool OpenFile(OpenAssetInfo info, List<string> flterStrings, List<string> excludeFiles)
		{
			string stackTrace = GetConsoleStackTrace();
			if (!string.IsNullOrEmpty(stackTrace) && stackTrace.ContainsAny(flterStrings))
			{
				System.Text.RegularExpressions.Match matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				while (matches.Success)
				{
					string pathLine = matches.Groups[1].Value;

					bool containsAny = pathLine.ContainsAny(excludeFiles);
					if (!containsAny)
					{
						int splitIndex = pathLine.LastIndexOf(":");
						string path = pathLine.Substring(0, splitIndex);
						int line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
						Object fileObject = AssetDatabase.LoadAssetAtPath<Object>(path);
						if (fileObject != null && AssetDatabase.OpenAsset(fileObject, line))
						{
							return true;
						}
						else
						{
							//string fullPath = FPlatformFileSystem.GetFullPath(path).ToPlatformStyle();
							//return FEditorUtility.OpenFileAtLineExternal(path, line);
						}

					}
					matches = matches.NextMatch();
				}
			}
			return false;
		}

		public override bool ProcessAssetOpen(OpenAssetInfo info)
		{
			if (info.assetPath.ContainsAny(msBlacklist))
			{
				if (isOpenningFile)
					return true;
				isOpenningFile = true;
				bool result = OpenFile(info, msFilterStrings, msBlacklist);
				isOpenningFile = false;
				return result;
			}
			return false;
		}
	}
}
