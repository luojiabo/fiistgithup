using System;
using System.Collections;

namespace Loki
{
	public enum ESystemStatus
	{
		NotExists,
		Uninitialized,
		Initialized,
		Startup,
		Shutdown,
	}

	public interface ISystem : IFixedUpdatable, IUpdatable, ILateUpdatable
	{
		string systemName { get; }
		IModuleInterface module { get; set; }
		IEnumerator Initialize();
		IEnumerator PostInitialize();
		void Uninitialize();
		void Startup();
		void Shutdown();
	}

	public static class ISystemExtension
	{
		public static TModuleInterface GetModule<TModuleInterface>(this ISystem system) where TModuleInterface : IModuleInterface
		{
			if (system.module is TModuleInterface)
				return (TModuleInterface)system.module;
			return default;
		}

		public static TSystem GetModuleSystem<TSystem>(this ISystem system, string otherSystemName) where TSystem : ISystem
		{
			if (system.module != null)
			{
				var sys = system.module.GetSystem(otherSystemName);
				if (sys == null)
					return default;
				DebugUtility.AssertFormat(sys is TSystem, "The system type [{0}] is not compatiable with type [{1}]", sys.GetType().Name, typeof(TSystem).Name);
				return (TSystem)sys;
			}
			return default;
		}

		public static TMostDerived GetModuleSystem<TMostDerived>(this ISystem system) where TMostDerived : ISystem
		{
			return GetModuleSystem<TMostDerived>(system, typeof(TMostDerived).Name);
		}

	}
}
