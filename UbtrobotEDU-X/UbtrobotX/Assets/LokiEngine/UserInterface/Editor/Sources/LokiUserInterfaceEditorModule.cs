using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Loki.UI
{
	[Module(engine = true, entryPoint = "Entry", order = 10100)]
	public class LokiUserInterfaceEditorModule : ModuleInterface<LokiUserInterfaceEditorModule>
	{
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
