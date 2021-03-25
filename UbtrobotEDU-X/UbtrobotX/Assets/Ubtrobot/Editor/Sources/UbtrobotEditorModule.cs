using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Loki;
using System.Threading.Tasks;

namespace Ubtrobot
{
	[Module(entryPoint = "Entry", order = 1)]
	public class UbtrobotEditorModule : ModuleInterface<UbtrobotEditorModule>
	{
		/// <summary>
		/// Do not declare InitializeOnLoadMethodAttribute On Generic 
		/// </summary>
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			

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
