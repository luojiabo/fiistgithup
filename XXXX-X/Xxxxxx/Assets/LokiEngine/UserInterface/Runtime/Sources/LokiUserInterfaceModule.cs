using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loki.UI
{
	[Module(engine = true, entryPoint = "Entry", order = 100)]
	public class LokiUserInterfaceModule : ModuleInterface<LokiUserInterfaceModule>
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
