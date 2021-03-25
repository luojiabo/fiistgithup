using System.Collections;

namespace Loki
{
	public enum EModuleStatus
	{
		None,
		Loaded,
		PreInitialize,
		Initialize,
		PostInitialize,
	}

	public enum EModuleShutdownReason
	{
		Runtime,
		Application,
	}

	public struct FModuleStatus
	{
		public string moduleName;
		public EModuleStatus status;
	}

	public interface IModuleInterface : IUpdatable, IFixedUpdatable, ILateUpdatable
	{
		string moduleName { get; }
		EModuleStatus status { get; set; }

		IEnumerator PreInitialize();
		IEnumerator Initialize();
		IEnumerator PostInitialize();
		void Uninitialize();
		void StartupModule();
		void ShutdownModule(EModuleShutdownReason reason);
		void OnApplicationQuit();
		ISystem GetSystem(string systemName);
		ISystem GetSystem(System.Type baseType);
	}
}
