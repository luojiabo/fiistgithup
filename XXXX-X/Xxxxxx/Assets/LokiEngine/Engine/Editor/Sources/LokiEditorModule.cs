using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Loki
{
	public class LokiEditorTime
	{
		private static double msEditorTime = 0.0;
		private static float msEditorDelta = 0.0f;
		public static DateTime editorStart { get; private set; }
		public static DateTime editorNow { get; private set; }
		public static double time
		{
			get
			{
				if (Application.isPlaying)
				{
					return Time.time;
				}
				return msEditorTime;
			}
		}
		public static float delta
		{
			get
			{
				if (Application.isPlaying)
				{
					return Time.deltaTime;
				}
				return msEditorDelta;
			}
		}

		public static void Reset()
		{
			editorNow = editorStart = DateTime.Now;
			msEditorTime = 0.0;
			msEditorDelta = 0.0f;
		}

		public static void Update()
		{
			editorNow = DateTime.Now;
			double currentTime = (editorNow - editorStart).TotalSeconds;
			msEditorDelta = (float)(currentTime - time);
			msEditorTime = currentTime;
		}
	}


	[Module(engine = true, entryPoint = "Entry", order = 10000)]
	public class LokiEditorModule : ModuleInterface<LokiEditorModule>
	{
		/// <summary>
		/// Do not declare InitializeOnLoadMethodAttribute On Generic 
		/// </summary>
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			// FDebug.LogTrace(FLoggerTags.Engine, "FLokiEditorModule.InitializeOnLoad");

			EditorChecker.InitializeOnLoad();

			LokiEditorApplication.InitializeOnLoad();

			//LoggerTags.CollectTags();
			LokiHierarchyWindow.InitializeOnLoad();

			AssetEditorManager.Get().Register(new ConsoleEditor());
		}

		private static void Entry()
		{
			RegisterThisModule();
		}

		public override void StartupModule()
		{
			base.StartupModule();
		}

		public override void ShutdownModule(EModuleShutdownReason reason)
		{
			base.ShutdownModule(reason);
		}
	}
}
