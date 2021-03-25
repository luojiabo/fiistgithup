using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Loki
{
	struct ModuleInfo
	{
		string originalFilename;
		string fileName;
	}

	public class ModuleManager
	{
		private readonly List<IModuleInterface> mAllModules = new List<IModuleInterface>(128);

		private static readonly ModuleManager msInstance;
		private ISystem mCachedSystem;

		public UEngine engine { get; private set; }

		public bool hasInitialized => engine != null;

		static ModuleManager()
		{
			msInstance = new ModuleManager();
		}

		public static ModuleManager Get()
		{
			return msInstance;
		}

		private ModuleManager()
		{

		}

		public Coroutine StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
				return null;
			return engine.StartCoroutine(enumerator);
		}

		public IEnumerator OnModuleInitialize(UEngine engine)
		{
			if (hasInitialized)
			{
				yield break;
			}
			this.engine = engine;

			DebugUtility.Log(LoggerTags.Engine, "ModuleManager.OnModuleInitialize: {0}", Time.realtimeSinceStartup);
			var co = StartCoroutine(PreInitialize());
			if (co != null)
				yield return co;
			co = StartCoroutine(Initialize());
			if (co != null)
				yield return co;
			co = StartCoroutine(PostInitialize());
			if (co != null)
				yield return co;
			DebugUtility.Log(LoggerTags.Engine, "ModuleManager.OnModuleInitialize done: {0}", Time.realtimeSinceStartup);
		}

		public IEnumerator PreInitialize()
		{
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.PreInitialize begin: {0}. Module Count : {1}", Time.realtimeSinceStartup, mAllModules.Count);
			//ProfilingUtility.BeginSample("ModuleManager.PreInitialize");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.Loaded)
				{
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.PreInitialize begin: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
					//ProfilingUtility.BeginSample("ModuleManager.PreInitialize_", m.moduleName);
					var coroutine = StartCoroutine(m.PreInitialize());
					if (coroutine != null)
						yield return coroutine;
					m.status = EModuleStatus.PreInitialize;
					//ProfilingUtility.EndSample();
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.PreInitialize end: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
				}
			}
			//ProfilingUtility.EndSample();
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.PreInitialize end");
		}

		public IEnumerator Initialize()
		{
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.Initialize begin: {0}. Module Count : {1}", Time.realtimeSinceStartup, mAllModules.Count);
			//ProfilingUtility.BeginSample("ModuleManager.Initialize");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.PreInitialize)
				{
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.Initialize begin: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
					//ProfilingUtility.BeginSample("ModuleManager.Initialize_", m.moduleName);
					var coroutine = StartCoroutine(m.Initialize());
					if (coroutine != null)
						yield return coroutine;
					m.status = EModuleStatus.Initialize;
					//ProfilingUtility.EndSample();
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.Initialize end: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
				}
			}
			//ProfilingUtility.EndSample();
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.Initialize end all");
		}

		public IEnumerator PostInitialize()
		{
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.PostInitialize begin: {0}. Module Count : {1}", Time.realtimeSinceStartup, mAllModules.Count);
			//ProfilingUtility.BeginSample("ModuleManager.PostInitialize");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.Initialize)
				{
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.PostInitialize begin: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
					//ProfilingUtility.BeginSample("ModuleManager.PostInitialize_", m.moduleName);
					var coroutine = StartCoroutine(m.PostInitialize());
					if (coroutine != null)
						yield return coroutine;
					m.status = EModuleStatus.PostInitialize;
					//ProfilingUtility.EndSample();
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.PostInitialize end: {0} - {1}", Time.realtimeSinceStartup, m.moduleName);
				}
			}
			//ProfilingUtility.EndSample();
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.PostInitialize end");
		}

		internal void FixedUpdate(float fixedDeltaTime)
		{
			ProfilingUtility.BeginSample("ModuleManager.FixedUpdate");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.PostInitialize)
				{
					ProfilingUtility.BeginSample("ModuleManager.FixedUpdate_", m.moduleName);
					try
					{
						m.OnFixedUpdate(fixedDeltaTime);
					}
					catch (System.Exception ex)
					{
						DebugUtility.LogException(ex);
					}
					ProfilingUtility.EndSample();
				}
			}
			ProfilingUtility.EndSample();
		}

		internal void Update(float deltaTime)
		{
			ProfilingUtility.BeginSample("ModuleManager.Update");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.PostInitialize)
				{
					ProfilingUtility.BeginSample("ModuleManager.Update_", m.moduleName);
					try
					{
						m.OnUpdate(deltaTime);
					}
					catch (System.Exception ex)
					{
						DebugUtility.LogException(ex);
					}
					ProfilingUtility.EndSample();
				}
			}
			ProfilingUtility.EndSample();
		}

		internal void OnLateUpdate()
		{
			ProfilingUtility.BeginSample("ModuleManager.LateUpdate");
			foreach (var m in mAllModules)
			{
				if (m.status == EModuleStatus.PostInitialize)
				{
					ProfilingUtility.BeginSample("ModuleManager.LateUpdate_", m.moduleName);
					try
					{
						m.OnLateUpdate();
					}
					catch (System.Exception ex)
					{
						DebugUtility.LogException(ex);
					}
					ProfilingUtility.EndSample();
				}
			}
			ProfilingUtility.EndSample();
		}

		internal void OnApplicationQuit()
		{
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.OnApplicationQuit begin all");
			ProfilingUtility.BeginSample("ModuleManager.OnApplicationQuit");
			for (int i = mAllModules.Count - 1; i >= 0; i--)
			{
				IModuleInterface m = mAllModules[i];
				DebugUtility.Log(LoggerTags.Module, "ModuleManager.OnApplicationQuit begin : " + m.moduleName);
				ProfilingUtility.BeginSample("ModuleManager.OnApplicationQuit_", m.moduleName);
				try
				{
					m.OnApplicationQuit();
				}
				catch (System.Exception ex)
				{
					DebugUtility.LogException(ex);
				}
				ProfilingUtility.EndSample();
				DebugUtility.Log(LoggerTags.Module, "ModuleManager.OnApplicationQuit end : " + m.moduleName);
			}
			ProfilingUtility.EndSample();
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.OnApplicationQuit end all");
		}

		internal void Uninitialize()
		{
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.Uninitialize begin all");
			ProfilingUtility.BeginSample("ModuleManager.Uninitialize");
			for (int i = mAllModules.Count - 1; i >= 0; i--)
			{
				IModuleInterface m = mAllModules[i];
				if (m.status == EModuleStatus.PostInitialize)
				{
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.Uninitialize begin : " + m.moduleName);
					ProfilingUtility.BeginSample("ModuleManager.Uninitialize_", m.moduleName);
					try
					{
						m.Uninitialize();
					}
					catch (System.Exception ex)
					{
						DebugUtility.LogException(ex);
					}
					m.status = EModuleStatus.Loaded;
					ProfilingUtility.EndSample();
					DebugUtility.Log(LoggerTags.Module, "ModuleManager.Uninitialize end : " + m.moduleName);
				}
			}
			ProfilingUtility.EndSample();
			DebugUtility.Log(LoggerTags.Module, "ModuleManager.Uninitialize end all");
		}

		public void ClearPendingCleanupObjects()
		{

		}

		internal IModuleInterface AddModule(IModuleInterface module)
		{
			if (module.status == EModuleStatus.None)
			{
				mAllModules.Add(module);
				module.StartupModule();
				module.status = EModuleStatus.Loaded;
				return module;
			}
			return null;
		}

		public void AbandonModule(string moduleName)
		{
			ProfilingUtility.BeginSample("ModuleManager.AbandonModule_", moduleName);
			IModuleInterface m = GetModule(moduleName);
			if (m != null)
			{
				if (m.status != EModuleStatus.None)
				{
					m.ShutdownModule(EModuleShutdownReason.Runtime);
					m.status = EModuleStatus.None;
				}
			}
			ProfilingUtility.EndSample();
		}

		public IModuleInterface GetModule(string moduleName)
		{
			foreach (var m in mAllModules)
			{
				if (m.moduleName == moduleName)
				{
					return m;
				}
			}
			return null;
		}

		public bool ModuleExists(string moduleName)
		{
			foreach (var m in mAllModules)
			{
				if (m.moduleName == moduleName)
				{
					return true;
				}
			}
			return false;
		}

		public bool ModuleExists<T>() where T : IModuleInterface
		{
			foreach (var m in mAllModules)
			{
				if (m is T)
				{
					return true;
				}
			}
			return false;
		}

		public T GetCompatiableModule<T>() where T : IModuleInterface
		{
			foreach (var m in mAllModules)
			{
				if (m is T)
				{
					return (T)m;
				}
			}
			return default(T);
		}

		public TModuleInterfaceMostDerived GetModule<TModuleInterfaceMostDerived>() where TModuleInterfaceMostDerived : IModuleInterface
		{
			return GetModule<TModuleInterfaceMostDerived>(typeof(TModuleInterfaceMostDerived).Name);
		}

		public T GetModule<T>(string moduleName) where T : IModuleInterface
		{
			foreach (var m in mAllModules)
			{
				if (m.moduleName == moduleName)
				{
					return (T)m;
				}
			}
			return default(T);
		}

		public bool IsModuleLoaded(string moduleName)
		{
			foreach (var m in mAllModules)
			{
				if (m.moduleName == moduleName)
				{
					return m.status != EModuleStatus.None;
				}
			}
			return false;
		}

		public bool QueryModule(string moduleName, ref FModuleStatus outStatus)
		{
			outStatus.moduleName = moduleName;
			outStatus.status = EModuleStatus.None;
			foreach (var m in mAllModules)
			{
				if (m.moduleName == moduleName)
				{
					outStatus.status = m.status;
					return true;
				}
			}
			return false;
		}

		public TModuleInterfaceMostDerived GetModuleChecked<TModuleInterfaceMostDerived>() where TModuleInterfaceMostDerived : ModuleInterface<TModuleInterfaceMostDerived>, new()
		{
			return GetModuleChecked<TModuleInterfaceMostDerived>(typeof(TModuleInterfaceMostDerived).Name);
		}

		public TModuleInterface GetModuleChecked<TModuleInterface>(string moduleName) where TModuleInterface : ModuleInterface<TModuleInterface>, new()
		{
			IModuleInterface module = GetModule(moduleName);
			if (module is TModuleInterface)
			{
				TModuleInterface result = (TModuleInterface)module;

				DebugUtility.AssertFormat(result != null, "Failded to call GetModuleChecked({0})", moduleName);
				DebugUtility.AssertFormat(result.status >= EModuleStatus.Initialize, "Failded to call GetModuleChecked({0}), please ensure that the module has initialized", moduleName);
				return result;
			}

			DebugUtility.AssertFormat(false, "Failded to call GetModuleChecked({0})", moduleName);
			return default(TModuleInterface);
		}

		public TSystem GetSystemChecked<TModuleInterface, TSystem>(string moduleName, string systemName)
			where TModuleInterface : ModuleInterface<TModuleInterface>, new()
			where TSystem : ISystem
		{
			return GetModuleChecked<TModuleInterface>(moduleName).GetSystem<TSystem>(systemName);
		}

		public TSystemMostDerived GetSystemChecked<TMostDerivedModuleInterface, TSystemMostDerived>()
			where TMostDerivedModuleInterface : ModuleInterface<TMostDerivedModuleInterface>, new()
			where TSystemMostDerived : ISystem
		{
			return GetModuleChecked<TMostDerivedModuleInterface>().GetSystem<TSystemMostDerived>();
		}

		public TSystemMostDerived GetSystemChecked<TSystemMostDerived>()
			where TSystemMostDerived : ISystem
		{
			return GetSystemChecked<TSystemMostDerived>(typeof(TSystemMostDerived).Name);
		}

		public TSystemMostDerived GetSystemChecked<TSystemMostDerived>(string systemName)
			where TSystemMostDerived : ISystem
		{
			// performance
			if ((mCachedSystem != null) && (mCachedSystem.module != null) && (mCachedSystem is TSystemMostDerived))
			{
				return (TSystemMostDerived)mCachedSystem;
			}

			foreach (var m in mAllModules)
			{
				if (m.status >= EModuleStatus.Loaded)
				{
					var sys = m.GetSystem(systemName);
					if (sys != null)
					{
						mCachedSystem = sys;
						return (TSystemMostDerived)sys;
					}
				}
			}
			return default;
		}

		public TSystem GetSystemChecked<TSystem>(System.Type baseType)
			where TSystem : ISystem
		{
			// performance
			if ((mCachedSystem != null) && (mCachedSystem.module != null) && (mCachedSystem is TSystem))
			{
				return (TSystem)mCachedSystem;
			}

			foreach (var m in mAllModules)
			{
				if (m.status >= EModuleStatus.Loaded)
				{
					var sys = m.GetSystem(baseType);
					if (sys != null && (sys is TSystem))
					{
						mCachedSystem = sys;
						return (TSystem)sys;
					}
				}
			}
			return default;
		}

		public int GetModuleCount()
		{
			return mAllModules.Count;
		}

		public void UnloadModulesAtShutdown()
		{
			ProfilingUtility.BeginSample("ModuleManager.UnloadModulesAtShutdown");
			foreach (var m in mAllModules)
			{
				if (m.status != EModuleStatus.None)
				{
					m.status = EModuleStatus.None;
					ProfilingUtility.BeginSample("ModuleManager.UnloadModules_", m.moduleName);
					m.ShutdownModule(EModuleShutdownReason.Application);
					ProfilingUtility.EndSample();
				}
			}
			ProfilingUtility.EndSample();
		}
	}
}
