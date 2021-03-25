using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[NameToType]
	public class SceneInfo : UComponent
	{
		public EnvRange config;
		public Transform modelHandle;
		public SphereCameraZone sphereCameraZone;
		private Environment defaultEnv;

		public Environment activeEnv
		{
			get
			{
				if (defaultEnv == null)
				{
					EnvironmentSystem environmentSystem = ModuleManager.Get().GetSystemChecked<EnvironmentSystem>();
					if (environmentSystem!=null)
					{
						defaultEnv = environmentSystem.defaultEnvironment;
					}
				}
				return defaultEnv;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}

		public void Initialize()
		{
			// todo
			OnInitialize();
			// todo
		}

		protected virtual void OnInitialize()
		{
			if (sphereCameraZone == null)
			{
				sphereCameraZone = GameObject.FindObjectOfType<SphereCameraZone>();
			}
		}

		public Vector3 GetModelHandlePosition(Vector3 offset)
		{
			return modelHandle != null ? modelHandle.position + offset : offset;
		}

		public Vector3 GetModelHandlePosition(float y)
		{
			return modelHandle != null ? modelHandle.position + new Vector3(0.0f, y, 0.0f) : new Vector3(0.0f, y, 0.0f);
		}

		public void LookAtModelHandle(Transform tr)
		{
			tr.LookAt(GetModelHandlePosition(new Vector3(0.0f, tr.position.y, 0.0f)), Vector3.up);
		}
	}
}
