using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	[CreateAssetMenu(menuName = "Loki/Configs/Editor/Editor Settings", fileName = "EditorSettings", order = -999)]
	public class EngineEditorSettings : EditorModuleSettings<EngineEditorSettings>
	{
		public override string category { get { return "EditorConfig"; } }

		public EngineEditorStylesSettings editorStylesConfig;
		public MacroSettings macroSettings;

		/// <summary>
		/// DO NOT Deal with the !UNITY_EDITOR, let it get some compile errors.
		/// </summary>
		/// <returns></returns>
		public static EngineEditorSettings GetOrLoad()
		{
			return GetOrLoad("EditorConfig/EditorSettings");
		}
	}
}
