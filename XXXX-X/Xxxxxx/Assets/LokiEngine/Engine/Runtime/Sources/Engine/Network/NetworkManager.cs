using System;
using System.Collections;
using System.Collections.Generic;

namespace Loki
{
	public abstract class NetworkManager<TMostDerived> : USingletonObject<TMostDerived>, ISystem where TMostDerived : NetworkManager<TMostDerived>
	{
		private static readonly Type msType = typeof(TMostDerived);

		public string systemName => msType.Name;

		public IModuleInterface module { get; set; }

		public virtual IEnumerator Initialize()
		{
			return null;
		}

		public virtual IEnumerator PostInitialize()
		{
			return null;
		}

		public virtual void Uninitialize()
		{
		}

		public virtual void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public virtual void OnLateUpdate()
		{
		}

		public virtual void OnUpdate(float deltaTime)
		{
		}

		public virtual void Shutdown()
		{
		}

		public virtual void Startup()
		{
		}
	}
}
