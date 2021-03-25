using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loki;

namespace Ubtrobot
{
	/// <summary>
	/// The Mechanical animals system 
	/// </summary>
	public class MechanicalAnimalsSystem : ISystem
	{
		private static readonly Type msType = typeof(MechanicalAnimalsSystem);

		private readonly TSafeForeachList<IMechanicalAnimator> mAllAnimators = new TSafeForeachList<IMechanicalAnimator>();

		public string systemName { get { return msType.Name; } }

		public IModuleInterface module { get; set; }

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
