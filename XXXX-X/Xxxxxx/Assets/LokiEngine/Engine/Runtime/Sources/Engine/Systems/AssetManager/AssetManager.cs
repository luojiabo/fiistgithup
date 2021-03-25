using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loki
{
	public partial class AssetManager : USingletonObject<AssetManager>, ISystem
	{
		private static readonly Type msType = typeof(AssetManager);

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
		public void Uninitialize()
		{

		}

		public void Startup()
		{

		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{

		}

		public void OnUpdate(float deltaTime)
		{
			DoInstantiateTask(deltaTime);
		}

		public void OnLateUpdate()
		{

		}

		public void Shutdown()
		{

		}
	}
}
