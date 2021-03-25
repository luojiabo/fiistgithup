using System;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Loki
{
	/// <summary>
	/// Please keep the *.uxml and the *.uss in the same directory of this file
	/// </summary>
	public class ProjectBuilder : LokiEditorWindow<ProjectBuilder>
	{
		private static string msStylePath = "";

		public override string assetName { get { return "ProjectBuilder"; } }


		[MenuItem("Loki/Window/ProjectBuilder/Window")]
		private static void ShowWindow()
		{
			ProjectBuilder wnd = GetWindow<ProjectBuilder>("ProjectBuilder");
		}

		[MenuItem("Loki/Window/ProjectBuilder/Location")]
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

			var packageName = rootVisualElement.Q<TextField>("packageName");
			if (packageName != null)
			{
				packageName.value = EditorUtilityHelper.GetPackageName();
				packageName.RegisterValueChangedCallback((e) =>
				{

				});
			}

			var applyPackage = rootVisualElement.Q<Button>("packageApply");
			if (applyPackage != null)
			{
				applyPackage.clicked += () =>
				{
					if (string.IsNullOrEmpty(packageName.value))
					{
						packageName.value = EditorUtilityHelper.GetPackageName();
					}
					else
					{
						EditorUtilityHelper.ApplyPackageName(packageName.value);
					}
				};
			}

			//var btnBuildWindowsDbg = rootVisualElement.Q<TextField>("buildWindowDebug");
			//var btnBuildWindowsRelease = rootVisualElement.Q<TextField>("buildWindowRelease");

			SetBuildPlatform("buildWindowDebug", () => BuildDebug(BuildTarget.StandaloneWindows64));
			SetBuildPlatform("buildWindowRelease", () => BuildRelease(BuildTarget.StandaloneWindows64));
			SetBuildPlatform("buildWebGLDebug", () => BuildDebug(BuildTarget.WebGL));
			SetBuildPlatform("buildWebGLRelease", () => BuildRelease(BuildTarget.WebGL));
		}

		private static void BuildDebug(BuildTarget target)
		{
			if (EditorUtility.DisplayDialog($"Build Package", $"Build Debug {target} ? ", "Build", "Cancel"))
			{
				var options = new BuildPlayerOptions();
				options.target = target;
				options.targetGroup = BuildPipeline.GetBuildTargetGroup(target);
				options.options = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
				options.scenes = EditorBuildSettings.scenes.ToArray((scene) => scene.enabled ? FileSystem.FullPathToAssetPath(scene.path) : null, v => !string.IsNullOrEmpty(v));
				DebugUtility.Log(LoggerTags.BuildSystem, string.Join(",", options.scenes));
				options.locationPathName = GetDebugExportPath(target, options.targetGroup);
				BuildPipeline.BuildPlayer(options);
			}
		}

		private static void BuildRelease(BuildTarget target)
		{
			if (EditorUtility.DisplayDialog($"Build Package", $"Build Release {target} ? ", "Build", "Cancel"))
			{
				var options = new BuildPlayerOptions();
				options.target = target;
				options.targetGroup = BuildPipeline.GetBuildTargetGroup(target);
				options.options = BuildOptions.None;
				options.scenes = EditorBuildSettings.scenes.ToArray((scene) => scene.enabled ? FileSystem.FullPathToAssetPath(scene.path) : null, v => !string.IsNullOrEmpty(v));
				DebugUtility.Log(LoggerTags.BuildSystem, string.Join(",", options.scenes));
				options.locationPathName = GetReleaseExportPath(target, options.targetGroup);
				BuildPipeline.BuildPlayer(options);
			}
		}

		private static string GetDebugExportPath(BuildTarget target, BuildTargetGroup targetGroup)
		{
			return string.Format("Export/{0}/Debug_{2}/{1}", target.ToString(), GeneratePlatformAppName(target, targetGroup), DateTime.Now.ToString("yyyy-MM-dd"));
		}

		private static string GetReleaseExportPath(BuildTarget target, BuildTargetGroup targetGroup)
		{
			return string.Format("Export/{0}/Release_{2}/{1}", target.ToString(), GeneratePlatformAppName(target, targetGroup), DateTime.Now.ToString("yyyy-MM-dd"));
		}

		private static string GeneratePlatformAppName(BuildTarget target, BuildTargetGroup group)
		{
			switch (target)
			{
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					{
						return string.Format("{1}_{0}/{1}.exe", 
							PlayerSettings.bundleVersion,
							PlayerSettings.productName);
						break;
					}
				case BuildTarget.Android:
					{
						return string.Format("{0}-{1}_{2}.{3}.apk",
							PlayerSettings.productName, 
							PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android),
							PlayerSettings.bundleVersion,
							PlayerSettings.Android.bundleVersionCode.ToString());
						break;
					}
				case BuildTarget.iOS:
					{
						return string.Format("{0}-{1}_{2}.{3}.ipa",
							PlayerSettings.productName,
							PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS), 
							PlayerSettings.bundleVersion,
							PlayerSettings.iOS.buildNumber);
						break;
					}
				case BuildTarget.WebGL:
					{
						return string.Format("{0}/{1}",
							PlayerSettings.bundleVersion,
							PlayerSettings.productName);
						break;
					}
			}
			throw new System.Exception("Unsupport " + target.ToString());
		}

		private Button SetBuildPlatform(string element, System.Action callback)
		{
			var buildBtn = rootVisualElement.Q<Button>(element);
			if (buildBtn != null)
			{
				buildBtn.clicked += callback;
			}
			return buildBtn;
		}
	}
}
