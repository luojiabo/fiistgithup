using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Loki
{
	[Module(engine = true, entryPoint = "Entry", order = 0)]
	public class LokiEngineModule : ModuleInterface<LokiEngineModule>
	{
		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Entry()
		{
			RegisterThisModule();
		}

		//public override async Task PreInitialize()
		//{
		//	var task = base.PreInitialize();
		//	if (task != null)
		//	{
		//		await task;
		//	}
		//}

		//public override async Task Initialize()
		//{
		//	var task = base.Initialize();
		//	if (task != null)
		//	{
		//		await task;
		//	}

		//}

		//public override async Task PostInitialize()
		//{
		//	var task = base.PostInitialize();
		//	if (task != null)
		//	{
		//		await task;
		//	}
		//}

		public override void StartupModule()
		{
			base.StartupModule();
			RegisterUSystem(ConsoleManager.GetOrAlloc());
			RegisterUSystem<PerformanceAnalytics>();
			RegisterUSystem<AssetManager>();
			RegisterUSystem<DebuggerManager>();
			RegisterUSystem<RenderSystem>();
			RegisterUSystem<ScreenSystem>();
		}

		public override void ShutdownModule(EModuleShutdownReason reason)
		{
			base.ShutdownModule(reason);
		}
	}
}
