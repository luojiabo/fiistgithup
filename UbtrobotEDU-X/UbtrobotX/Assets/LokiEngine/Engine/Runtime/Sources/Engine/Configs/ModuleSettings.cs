// #define DISABLE_ASSETDATABASE_TEST

using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public abstract class ModuleSettings : UAssetObject
	{
		public override bool isSubAsset { get { return false; } }

	}

	public abstract class EditorModuleSettings : ModuleSettings
	{

	}

	//[CreateAssetMenu(menuName = "Loki/Configs/Module Config", fileName = "ModuleConfig", order = -999)]
	public abstract class ModuleSettings<T> : ModuleSettings where T : ModuleSettings<T>
	{
		private static T msModuleAsset;

		public static T GetOrLoad(string address)
		{
			if (msModuleAsset == null)
			{
#if UNITY_EDITOR && !DISABLE_ASSETDATABASE_TEST
				if (!Application.isPlaying)
				{
					msModuleAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(
						FileSystem.Get().GetAssetPathCheck(EFilePathType.EngineGeneratedConfigPath, string.Concat(address, ".asset"), true));
					return msModuleAsset;
				}
#endif
				if (PreloadManager.GetOrAlloc() == null)
					return null;
				msModuleAsset = PreloadManager.GetOrAlloc().GetObject<T>(address);
			}
			return msModuleAsset;
		}
	}

	public abstract class EditorModuleSettings<T> : EditorModuleSettings where T : EditorModuleSettings<T>
	{
		private static T msModuleAsset = null;

		public static T GetOrLoad(string address)
		{
			if (msModuleAsset == null)
			{
#if UNITY_EDITOR
				msModuleAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(
					FileSystem.Get().GetAssetPathCheck(EFilePathType.EngineGeneratedConfigPath, string.Concat(address, ".asset"), true));
#endif
			}
			return msModuleAsset;
		}
	}
}
