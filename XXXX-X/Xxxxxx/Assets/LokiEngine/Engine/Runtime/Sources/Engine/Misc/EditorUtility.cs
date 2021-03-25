#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Loki
{
	public class EditorUtilityHelper
	{
		public static string GetPackageName()
		{
			return string.Concat("com.", PlayerSettings.companyName, ".", PlayerSettings.productName);
		}

		public static bool ApplyPackageName(string packageName, string channel = "")
		{
			packageName = packageName.Replace(" ", "");
			if (string.IsNullOrEmpty(packageName))
			{
				DebugUtility.LogError(LoggerTags.Engine, "The Package Name must be 'com.company.productName'");
				return false;
			}
			var infos = packageName.Split('.');
			if (infos.Length < 3)
			{
				DebugUtility.LogError(LoggerTags.Engine, "The Package Name must be 'com.company.productName'");
				return false;
			}

			foreach (var item in infos)
			{
				if (string.IsNullOrEmpty(item))
				{
					DebugUtility.LogError(LoggerTags.Engine, "The Package Name must be 'com.company.productName'");
					return false;
				}
			}

			if (infos[0] != "com")
			{
				DebugUtility.LogError(LoggerTags.Engine, "The Package Name must be 'com.company.productName'");
				return false;
			}

			PlayerSettings.companyName = infos[1];
			PlayerSettings.productName = infos[2];
			if (channel == null)
			{
				channel = "";
			}
			if (channel.Length > 0 && !channel.StartsWith("."))
			{
				channel = string.Concat(".", channel);
			}
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, packageName + channel);
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName + channel);
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, packageName + channel);
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, packageName + channel);
			return true;
		}

		public static void LocateInProjectPanel(string path)
		{
			EditorUtility.FocusProjectWindow();
			var obj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(FileSystem.FullPathToAssetPath(path));
			if (obj != null)
			{
				Selection.activeObject = obj;
			}
		}
	}
}
#endif
