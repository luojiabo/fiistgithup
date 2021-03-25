using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace Loki
{
	public partial class AssetManager
	{
#if UNITY_EDITOR
		public static bool OpenAssetInEditor(string assetPath)
		{
			Object fileObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
			if (fileObject != null && AssetDatabase.OpenAsset(fileObject, 0))
			{
				return true;
			}
			return false;
		}
#endif

		public static T LoadFromAssetsDatabase<T>(string path) where T : Object
		{
#if UNITY_EDITOR
			return AssetDatabase.LoadAssetAtPath<T>(path);
#else
			DebugUtility.LogErrorTrace(LoggerTags.AssetManager, "Please do not use LoadFromAssetsDatabase at runtime : {0}", path);
			return null;
#endif
		}

#if UNITY_EDITOR
		public static List<T> LoadAllAssetsForDirectory<T>(string path, string searchPattern = "*.prefab", SearchOption searchOption = SearchOption.AllDirectories) where T : Object
		{
			var files = Directory.GetFiles(path, searchPattern, searchOption);
			List<T> result = new List<T>();
			foreach (var file in files)
			{
				string assetPath = file.ToUNIXStyle().Replace(FileSystem.Get().projectPathWithAlt, string.Empty);
				T t =  AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if (t != null)
				{
					result.Add(t);
				}
			}
			return result;
		}
#endif
	}
}
