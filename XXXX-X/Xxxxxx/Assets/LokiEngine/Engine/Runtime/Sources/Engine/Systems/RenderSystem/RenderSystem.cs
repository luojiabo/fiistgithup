using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Loki
{
	public class RenderSystem : USingletonObject<RenderSystem>, ISystem
	{
		private static readonly Type msType = typeof(RenderSystem);

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

		private void CheckScreenSizeChanged()
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
