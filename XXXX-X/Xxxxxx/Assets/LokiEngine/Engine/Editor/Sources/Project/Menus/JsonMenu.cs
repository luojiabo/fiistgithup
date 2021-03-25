using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Loki.Json
{
	public class JsonMenu
	{
		[MenuItem("Assets/Json/Format")]
		private static void FormatJsons()
		{
			RewriteJson(true);
		}

		[MenuItem("Assets/Json/Compress")]
		private static void CompressJsons()
		{
			RewriteJson(false);
		}

		private static void RewriteJson(bool format)
		{
			var objects = Selection.objects;
			if (objects == null || objects.Length == 0)
				return;

			foreach (var selected in objects)
			{
				try
				{
					string assetPath = AssetDatabase.GetAssetPath(selected);
					//TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
					if (selected is TextAsset textAsset)
					{
						var text = textAsset.text;
						if (!string.IsNullOrEmpty(text))
						{
							var json = JsonMapper.ToObject(text);
							if (json != null)
							{
								JsonWriter jsonWriter = new JsonWriter();
								if (format)
								{
									jsonWriter.IndentValue = 4;
									jsonWriter.PrettyPrint = true;
								}
								json.ToJson(jsonWriter);
								File.WriteAllText(FileSystem.AssetPathToFullPath(assetPath), jsonWriter.ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
				}
			}

			AssetDatabase.Refresh();
		}
	}
}
