using System;
using UnityEngine;
using Loki;
using System.Threading.Tasks;
using Loki.UI;

namespace Ubtrobot
{
	[Module(entryPoint = "Entry", order = 0)]
	public class UbtrobotModule : ModuleInterface<UbtrobotModule>
	{
		private static void Entry()
		{
			RegisterThisModule();
		}

		public override void StartupModule()
		{
			base.StartupModule();

			RegisterUSystem<NetworkManager>();
			RegisterUSystem<Selection>();
			RegisterUSystem<WindowManager>();
			RegisterUSystem<MaterialManager>();

			RegisterSystem<InputSystem>();
			RegisterSystem<CameraSystem>();
			RegisterSystem<MechanicalAnimalsSystem>();
			//RegisterSystem<LocomotionSystem>();

			RegisterUSystem<RobotManager>();
			RegisterUSystem<EnvironmentSystem>();

			RegisterUSystem<TestSystem>();
		}

		public override void ShutdownModule(EModuleShutdownReason reason)
		{
			base.ShutdownModule(reason);
		}
	}
}
