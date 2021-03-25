using System;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Loki
{
	/// <summary>
	/// Please keep the *.uxml and the *.uss in the same directory of this file
	/// </summary>
	public class AssetBundleViewer : LokiEditorWindow<AssetBundleViewer>
	{
		private static string msStylePath = "";

		public override string assetName { get { return "AssetBundleViewer"; } }


		[MenuItem("Loki/Window/AssetBundleViewer/Window")]
		private static void ShowWindow()
		{
			ProjectBuilder wnd = GetWindow<ProjectBuilder>("AssetBundleViewer");
		}

		[MenuItem("Loki/Window/AssetBundleViewer/Location")]
		private static void LocateDirectory()
		{
			string current = GetCurrentDirectory();
			EditorUtilityHelper.LocateInProjectPanel(current);
		}

		public static string GetCurrentDirectory()
		{
			if (string.IsNullOrEmpty(msStylePath))
			{
				msStylePath = StacktraceUtility.GetCurrentDirectory(false);
			}
			return msStylePath;
		}

		protected override string GetAssetPath(string fileName)
		{
			return string.Concat(GetCurrentDirectory(), "/", fileName);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

		}
	}
}
