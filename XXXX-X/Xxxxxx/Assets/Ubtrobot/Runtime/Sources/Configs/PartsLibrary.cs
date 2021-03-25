#define ENABLE_PARTS_PREFAB_LINK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Parts Library", fileName = "PartsLibrary", order = -999)]
	public class PartsLibrary : UAssetObject
	{
		[AssetPathToObject]
		public string partLibrary = "Assets/Updatable/PartLibrary";

		[SerializeField]
		public List<GameObject> allParts;

		public bool enablePartsPrefabLink = true;

		public GameObject Instantiate(string partName)
		{
			if (allParts == null)
				return null;

			GameObject go = allParts.Find(temp => temp.name == partName);
			if (go != null)
			{
#if UNITY_EDITOR && ENABLE_PARTS_PREFAB_LINK
				if (!Application.isPlaying && enablePartsPrefabLink)
				{
					go = PrefabUtility.InstantiatePrefab(go) as GameObject;
				}
				else
				{
					go = Instantiate(go);
					go.name = partName;
					go.DestroyEditorOnly();
				}
#else
				go = Instantiate(go);
				go.name = partName;
				go.DestroyEditorOnly();
#endif
			}
			return go;
		}

#if UNITY_EDITOR
		public GameObject LoadPrefab(string partName)
		{
			if (allParts == null)
				return null;

			GameObject go = allParts.Find(temp => temp.name == partName);
			if (go != null)
			{
				//string assetPath = AssetDatabase.GetAssetPath(go);
				return (GameObject)PrefabUtility.InstantiatePrefab(go);
				//go = Instantiate(go);
				//go.name = partName;
			}
			return go;
		}

		[InspectorMethod]
		private void ScanParts()
		{
			string fullPath = FileSystem.GetFullPath(partLibrary);
			//var files = System.IO.Directory.GetFiles(partLibrary, "*.prefab", System.IO.SearchOption.AllDirectories);
			//var result = new List<GameObject>();
			//foreach (var file in files)
			//{
			//	string assetPath = file.ToUNIXStyle().Replace(FileSystem.Get().projectPathWithAlt, string.Empty);
			//	var t = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			//	if (t != null)
			//	{
			//		result.Add(t);
			//	}
			//}
			//allParts = result;
			allParts = AssetManager.LoadAllAssetsForDirectory<GameObject>(fullPath);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}
