using System;
using System.Collections.Generic;
using System.Linq;
using Loki;

namespace Ubtrobot
{
	public sealed class CameraSystem : Loki.CameraSystem<CameraSystem>
	{
		public override void OnFixedUpdate(float fixedDeltaTime)
		{
			base.OnFixedUpdate(fixedDeltaTime);
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
		}

		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}

		public override void Shutdown()
		{
			base.Shutdown();
		}

		public override void Startup()
		{
			base.Startup();
		}

		public override void Uninitialize()
		{
			base.Uninitialize();
		}
	}
}
