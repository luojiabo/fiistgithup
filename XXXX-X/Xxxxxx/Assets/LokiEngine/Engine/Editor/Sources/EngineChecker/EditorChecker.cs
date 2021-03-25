using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Loki
{
	public class EditorChecker
	{
		public static void InitializeOnLoad()
		{
			//FDebug.Log(FLoggerTags.Engine, "FEditorChecker.InitializeOnLoad");
			CheckEngineConfig();
			//AssetDatabase.SaveAssets();
		}

		public static void CheckEngineConfig()
		{
			var moduleConfigs = GlobalReflectionCache.FindTypes(typeof(ModuleSettings), false);
			if (moduleConfigs == null)
			{
				return;
			}

			foreach (var config in moduleConfigs)
			{
				var getOrLoadMethod = config.GetMethod("GetOrLoad");
				if (getOrLoadMethod == null)
				{
					DebugUtility.LogErrorTrace(LoggerTags.Engine, "The {0} must implement the static Method [public static ConfigSettingsType GetOrLoad()]", config.Name);
					continue;
				}

				string ModuleSettingsPath = null;
				ModuleSettings settings = getOrLoadMethod.Invoke(null, null) as ModuleSettings;
				if (settings == null)
				{
					settings = ScriptableObject.CreateInstance(config) as ModuleSettings;
					if (settings == null)
					{
						DebugUtility.LogErrorTrace(LoggerTags.Engine, "The {0} must implement the static Method [public static From_ModuleSettings GetOrLoad()]", config.Name);
						continue;
					}

					EFilePathType pathType = EFilePathType.EngineGeneratedConfigPath;// settings is EditorModuleSettings ? EFilePathType.EditorGeneratedConfigPath : EFilePathType.EngineGeneratedConfigPath;
					string path = FileSystem.Get().GetAssetPathCheck(pathType, settings.GetAssetFileName(), true);
					AssetDatabase.CreateAsset(settings, path);
					settings.OnCreated();

					ModuleSettingsPath = path;
				}
				else
				{
					EFilePathType pathType = EFilePathType.EngineGeneratedConfigPath;// settings is EditorModuleSettings ? EFilePathType.EditorGeneratedConfigPath : EFilePathType.EngineGeneratedConfigPath;
					ModuleSettingsPath = FileSystem.Get().GetAssetPathCheck(pathType, settings.GetAssetFileName(), true);
				}

				bool reimport = false;
				if (settings != null)
				{
					UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(ModuleSettingsPath);
					FieldInfo[] fieldInfos = config.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					foreach (var subAsset in subAssets)
					{
						bool isValid = false;
						if (subAsset != null)
						{
							if (subAsset.GetType() == settings.GetType())
							{
								isValid = true;
								continue;
							}

							foreach (var info in fieldInfos)
							{
								Type fieldType = info.FieldType;
								if (fieldType.IsSubclassOf(typeof(UAssetObject)) && !fieldType.IsAbstract)
								{
									if (subAsset.GetType() == fieldType)
									{
										isValid = true;

										object fieldValue = info.GetValue(settings);
										if (fieldValue == null)
										{
											info.SetValue(settings, subAsset);
										}
										break;
									}
								}
							}
						}

						if (!isValid)
						{
							AssetDatabase.RemoveObjectFromAsset(subAsset);
							UnityEngine.Object.DestroyImmediate(subAsset, true);
							reimport = true;
						}
					}

					foreach (var info in fieldInfos)
					{
						if (info.IsPublic || info.GetCustomAttribute<SerializableAttribute>() != null)
						{
							Type fieldType = info.FieldType;
							if (fieldType.IsSubclassOf(typeof(UAssetObject)) && !fieldType.IsAbstract)
							{
								object fieldValue = info.GetValue(settings);
								if (fieldValue == null)
								{
									reimport = true;
									UAssetObject asset = asset = ScriptableObject.CreateInstance(fieldType) as UAssetObject;
									if (asset != null)
									{
										asset.name = asset.GetType().Name;
										info.SetValue(settings, asset);

										if (!asset.isSubAsset)
										{
											EFilePathType pathType = EFilePathType.EngineGeneratedConfigPath; // asset is EditorModuleSettings ? EFilePathType.EditorGeneratedConfigPath : EFilePathType.EngineGeneratedConfigPath;
											string path = FileSystem.Get().GetAssetPathCheck(pathType, asset.GetAssetFileName(), true);
											AssetDatabase.CreateAsset(asset, path);
										}
										else
										{
											AssetDatabase.AddObjectToAsset(asset, ModuleSettingsPath);
										}
										settings.OnCreated();
									}
								}
							}
						}
					}
				}

				if (reimport)
				{
					AssetDatabase.ImportAsset(ModuleSettingsPath, ImportAssetOptions.Default);
				}
			}

		}
	}
}
