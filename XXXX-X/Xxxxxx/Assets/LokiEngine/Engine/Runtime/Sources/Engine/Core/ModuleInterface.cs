using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public abstract class ModuleInterface<T> : IModuleInterface where T : ModuleInterface<T>, new()
	{
		private static readonly Type msInterfaceType = typeof(T);
		private EModuleStatus mStatus = EModuleStatus.None;
		private readonly List<ISystem> mModuleSystems = new List<ISystem>();

		public string moduleName { get { return msInterfaceType.Name; } }
		public EModuleStatus status { get { return mStatus; } set { mStatus = value; } }

		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		protected static void RegisterThisModule()
		{
			ModuleManager mgr = ModuleManager.Get();
			if (mgr.ModuleExists(msInterfaceType.Name))
			{
				return;
			}
			mgr.AddModule(new T());
		}

		public ISystem GetSystem(string systemName)
		{
			ISystem system = mModuleSystems.Find(sys => sys.systemName == systemName);

			return system;
		}

		public ISystem GetSystem(System.Type baseType)
		{
			ISystem system = mModuleSystems.Find(sys => sys.GetType().IsSubclassOf(baseType));

			return system;
		}

		public TMostDerived GetSystem<TMostDerived>() where TMostDerived : ISystem
		{
			return (TMostDerived)GetSystem(typeof(TMostDerived).Name);
		}

		public TSystem GetSystem<TSystem>(string systemName) where TSystem : ISystem
		{
			return (TSystem)GetSystem(systemName);
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void UnregisterSystem(ISystem system)
		{
			mModuleSystems.Remove(system);
			system.module = null;
		}


		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void UnregisterSystem(string systemName)
		{
			var sys = GetSystem(systemName);
			if (sys != null)
			{
				UnregisterSystem(sys);
			}
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterSystem(ISystem system)
		{
			mModuleSystems.Add(system);
			system.module = this;
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterUSystem(Type type)
		{
#if UNITY_EDITOR
			if (!type.IsImplementOf<ISystem>(false))
			{
				return;
			}
#endif
			RegisterSystem((ISystem)UObject.NewImmortalObject(type, type.Name, typeof(T).Name));
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterUSystem<TMostDerived>(TMostDerived system) where TMostDerived : UObject, ISystem
		{
			system.AsImmortal(typeof(T).Name);
			RegisterSystem(system);
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterUSystem<TMostDerived>() where TMostDerived : UObject, ISystem
		{
			RegisterUSystem<TMostDerived>(true);
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterUSystem<TMostDerived>(bool autoSearchInst) where TMostDerived : UObject, ISystem
		{
			if (autoSearchInst)
			{
				var inst = UnityEngine.Object.FindObjectOfType<TMostDerived>();
				if (inst != null)
				{
					RegisterUSystem(inst);
					return;
				}
			}

			RegisterSystem(UObject.NewImmortalObject<TMostDerived>(typeof(TMostDerived).Name, typeof(T).Name));
		}

		/// <summary>
		/// Call at PreInitialize()
		/// </summary>
		protected void RegisterSystem<TMostDerived>() where TMostDerived : ISystem, new()
		{
			RegisterSystem(new TMostDerived());
		}

		public virtual void StartupModule()
		{
			foreach (var sys in mModuleSystems)
			{
				sys.Startup();
			}
		}

		public virtual void ShutdownModule(EModuleShutdownReason reason)
		{
			foreach (var sys in mModuleSystems)
			{
				sys.Shutdown();
			}
		}

		public virtual IEnumerator PreInitialize()
		{
			yield break;
		}

		public Coroutine StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
				return null;
			return ModuleManager.Get().StartCoroutine(enumerator);
		}

		public virtual void Uninitialize()
		{

		}

		public virtual IEnumerator Initialize()
		{
			var mm = ModuleManager.Get();
			foreach (var sys in mModuleSystems)
			{
				DebugUtility.Log(LoggerTags.Engine, "Initialize System {0}", sys.systemName);
				var coroutine = mm.StartCoroutine(sys.Initialize());
				if (coroutine != null)
				{
					yield return coroutine;
					DebugUtility.Log(LoggerTags.Engine, "Initialize System {0} done", sys.systemName);
				}
			}
		}

		public virtual IEnumerator PostInitialize()
		{
			var mm = ModuleManager.Get();
			foreach (var sys in mModuleSystems)
			{
				DebugUtility.Log(LoggerTags.Engine, "PostInitialize System {0}", sys.systemName);
				var coroutine = mm.StartCoroutine(sys.PostInitialize());
				if (coroutine != null)
				{
					yield return coroutine;
					DebugUtility.Log(LoggerTags.Engine, "PostInitialize System {0} done", sys.systemName);
				}
			}
		}

		public virtual void OnFixedUpdate(float fixedDeltaTime)
		{
			foreach (var sys in mModuleSystems)
			{
				sys.OnFixedUpdate(fixedDeltaTime);
			}
		}

		public virtual void OnUpdate(float deltaTime)
		{
			foreach (var sys in mModuleSystems)
			{
				sys.OnUpdate(deltaTime);
			}
		}

		public virtual void OnLateUpdate()
		{
			foreach (var sys in mModuleSystems)
			{
				sys.OnLateUpdate();
			}
		}

		public virtual void OnApplicationQuit()
		{
			ShutdownModule(EModuleShutdownReason.Application);
		}
	}
}
