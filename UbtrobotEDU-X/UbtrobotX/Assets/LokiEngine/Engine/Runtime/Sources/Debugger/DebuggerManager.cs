using System;
using System.Collections;
using System.Collections.Generic;

namespace Loki
{
	public class DebuggerManager : USingletonObject<DebuggerManager>, ISystem
	{
		private static readonly Type msType = typeof(DebuggerManager);
		public IModuleInterface module { get; set; }

		public string systemName { get { return msType.Name; } }

		public IEnumerator Initialize()
		{
			return null;
		}

		public IEnumerator PostInitialize()
		{
			return null;
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}

		public void OnUpdate(float deltaTime)
		{
		}

		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		public void Uninitialize()
		{
		}
	}
}
