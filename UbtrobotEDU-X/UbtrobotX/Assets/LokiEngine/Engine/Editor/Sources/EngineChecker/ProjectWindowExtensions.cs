using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Loki
{
	public class ProjectWindowExtensions
	{
		private static void Delete(IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				Delete(path);
			}
		}

		private static void Delete(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				else if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

#if UNITY_EDITOR_WIN
		[MenuItem("Assets/Clear Solution")]
		private static void ClearSolution()
		{
			string path = Path.Combine(Application.dataPath, "..");
			Delete(Directory.GetFiles(path, "*.csproj"));
			Delete(Directory.GetFiles(path, "*.sln"));
			Delete(Path.Combine(path, ".vs"));
		}

		[MenuItem("Assets/Generate Solution")]
		private static void GenerateSolution()
		{
			ClearSolution();
			EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
		}
#endif
	}
}
