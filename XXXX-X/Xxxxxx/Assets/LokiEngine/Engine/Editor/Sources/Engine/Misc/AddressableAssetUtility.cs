#if UNITY_ADDRESSABLE_SYSTEM
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Loki
{
	using Object = UnityEngine.Object;

	public	static class AddressableAssetUtility
	{
		public static class CommonStrings
		{
			public const string UnityEditorResourcePath = "library/unity editor resources";
			public const string UnityDefaultResourcePath = "library/unity default resources";
			public const string UnityBuiltInExtraPath = "resources/unity_builtin_extra";
			//public const string AssetBundleNameFormat = "archive:/{0}/{0}";
			//public const string SceneBundleNameFormat = "archive:/{0}/{1}.sharedAssets";
		}


		internal static bool IsInResources(string path)
		{
			return path.Replace('\\', '/').ToLower().Contains("/resources/");
		}
		internal static bool GetPathAndGUIDFromTarget(Object t, out string path, ref string guid, out Type mainAssetType)
		{
			mainAssetType = null;
			path = AssetDatabase.GetAssetOrScenePath(t);
			if (!IsPathValidForEntry(path))
				return false;
			guid = AssetDatabase.AssetPathToGUID(path);
			if (String.IsNullOrEmpty(guid))
				return false;
			mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);
			if (mainAssetType != t.GetType() && !typeof(AssetImporter).IsAssignableFrom(t.GetType()))
				return false;
			return true;
		}

		
		internal static bool IsPathValidForEntry(string path)
		{
			if (String.IsNullOrEmpty(path))
				return false;
			path = path.ToLower();
			if (path == CommonStrings.UnityEditorResourcePath ||
				path == CommonStrings.UnityDefaultResourcePath ||
				path == CommonStrings.UnityBuiltInExtraPath)
				return false;
			var ext = Path.GetExtension(path);
			if (ext == ".cs" || ext == ".js" || ext == ".boo" || ext == ".exe" || ext == ".dll")
				return false;
			var t = AssetDatabase.GetMainAssetTypeAtPath(path);
			if (t != null && BuildUtility.IsEditorAssembly(t.Assembly))
				return false;
			return true;
		}
		

		internal static void ConvertAssetBundlesToAddressables()
		{
			AssetDatabase.RemoveUnusedAssetBundleNames();
			var bundleList = AssetDatabase.GetAllAssetBundleNames();

			float fullCount = bundleList.Length;
			int currCount = 0;

			var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
			foreach (var bundle in bundleList)
			{
				if (EditorUtility.DisplayCancelableProgressBar("Converting Legacy Asset Bundles", bundle, currCount / fullCount))
					break;

				currCount++;
				var group = settings.CreateGroup(bundle, false, false, false, null);
				var schema = @group.AddSchema<BundledAssetGroupSchema>();
				schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
				schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
				schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
				@group.AddSchema<ContentUpdateGroupSchema>().StaticContent = true;

				var assetList = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);

				foreach (var asset in assetList)
				{
					var guid = AssetDatabase.AssetPathToGUID(asset);
					settings.CreateOrMoveEntry(guid, @group, false, false);
					var imp = AssetImporter.GetAtPath(asset);
					if (imp != null)
						imp.SetAssetBundleNameAndVariant(String.Empty, String.Empty);
				}
			}

			if (fullCount > 0)
				settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
			EditorUtility.ClearProgressBar();
			AssetDatabase.RemoveUnusedAssetBundleNames();
		}

		/// <summary>
		/// Get all types that can be assigned to type T
		/// </summary>
		/// <typeparam name="T">The class type to use as the base class or interface for all found types.</typeparam>
		/// <returns>A list of types that are assignable to type T.  The results are cached.</returns>
		public static List<Type> GetTypes<T>()
		{
			return TypeManager<T>.Types;
		}

		/// <summary>
		/// Get all types that can be assigned to type rootType.
		/// </summary>
		/// <param name="rootType">The class type to use as the base class or interface for all found types.</param>
		/// <returns>A list of types that are assignable to type T.  The results are not cached.</returns>
		public static List<Type> GetTypes(Type rootType)
		{
			return TypeManager.GetManagerTypes(rootType);
		}

		class TypeManager
		{
			public static List<Type> GetManagerTypes(Type rootType)
			{
				var types = new List<Type>();
				try
				{
					foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
					{
						if (a.IsDynamic)
							continue;
						foreach (var t in a.ExportedTypes)
						{
							if (t != rootType && rootType.IsAssignableFrom(t) && !t.IsAbstract)
								types.Add(t);
						}
					}
				}
				catch (Exception)
				{
					// ignored
				}

				return types;
			}
		}

		class TypeManager<T> : TypeManager
		{
			// ReSharper disable once StaticMemberInGenericType
			static List<Type> s_Types;
			public static List<Type> Types
			{
				get
				{
					if (s_Types == null)
						s_Types = GetManagerTypes(typeof(T));

					return s_Types;
				}
			}
		}

		internal static bool SafeMoveResourcesToGroup(AddressableAssetSettings settings, AddressableAssetGroup targetGroup, List<string> paths)
		{
			var guids = new List<string>();
			foreach (var p in paths)
			{
				guids.Add(AssetDatabase.AssetPathToGUID(p));
			}
			return SafeMoveResourcesToGroup(settings, targetGroup, paths, guids);
		}
		internal static bool SafeMoveResourcesToGroup(AddressableAssetSettings settings, AddressableAssetGroup targetGroup, List<string> paths, List<string> guids)
		{
			if (guids == null || guids.Count == 0 || paths == null || guids.Count != paths.Count)
			{
				DebugUtility.LogWarning(LoggerTags.Engine, "No valid Resources found to move");
				return false;
			}

			if (targetGroup == null)
			{
				DebugUtility.LogWarning(LoggerTags.Engine, "No valid group to move Resources to");
				return false;
			}

			Dictionary<string, string> guidToNewPath = new Dictionary<string, string>();

			var message = "Any assets in Resources that you wish to mark as Addressable must be moved within the project. We will move the files to:\n\n";
			for (int i = 0; i < guids.Count; i++)
			{
				var newName = paths[i].Replace("\\", "/");
				newName = newName.Replace("Resources", "Resources_moved");
				newName = newName.Replace("resources", "resources_moved");
				if (newName == paths[i])
					continue;

				guidToNewPath.Add(guids[i], newName);
				message += newName + "\n";
			}
			message += "\nAre you sure you want to proceed?";
			if (EditorUtility.DisplayDialog("Move From Resources", message, "Yes", "No"))
			{
				MoveAssetsFromResources(guidToNewPath, targetGroup,settings);
				return true;
			}
			return false;
		}

		internal static void MoveAssetsFromResources(Dictionary<string, string> guidToNewPath, AddressableAssetGroup targetParent, AddressableAssetSettings settings)
		{
			if (guidToNewPath == null)
				return;
			var entries = new List<AddressableAssetEntry>();
			AssetDatabase.StartAssetEditing();
			foreach (var item in guidToNewPath)
			{

				var dirInfo = new FileInfo(item.Value).Directory;
				if (dirInfo != null && !dirInfo.Exists)
				{
					dirInfo.Create();
					AssetDatabase.StopAssetEditing();
					AssetDatabase.Refresh();
					AssetDatabase.StartAssetEditing();
				}

				var oldPath = AssetDatabase.GUIDToAssetPath(item.Key);
				var errorStr = AssetDatabase.MoveAsset(oldPath, item.Value);
				if (!string.IsNullOrEmpty(errorStr))
				{
					DebugUtility.LogError(LoggerTags.Engine, "Error moving asset:{0} ", errorStr);
				}
				else
				{
					AddressableAssetEntry e = settings.FindAssetEntry(item.Key);
					if (e != null)
						e.IsInResources = false;

					var newEntry = settings.CreateOrMoveEntry(item.Key, targetParent, false, false);
					var index = oldPath.ToLower().LastIndexOf("resources/");
					if (index >= 0)
					{
						var newAddress = Path.GetFileNameWithoutExtension(oldPath.Substring(index + 10));
						if (!string.IsNullOrEmpty(newAddress))
						{
							newEntry.address = newAddress;
						}
					}
					entries.Add(newEntry);
				}

			}
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entries, true, true);
		}

		static Dictionary<Type, string> s_CachedDisplayNames = new Dictionary<Type, string>();
		internal static string GetCachedTypeDisplayName(Type type)
		{
			string result = "<none>";
			if (type != null)
			{
				if (!s_CachedDisplayNames.TryGetValue(type, out result))
				{
					var displayNameAtr = type.GetCustomAttribute<DisplayNameAttribute>();
					if (displayNameAtr != null)
					{
						result = (string)displayNameAtr.DisplayName;
					}
					else
						result = type.Name;

					s_CachedDisplayNames.Add(type, result);
				}
			}
			return result;
		}
	}

	
}
#endif

