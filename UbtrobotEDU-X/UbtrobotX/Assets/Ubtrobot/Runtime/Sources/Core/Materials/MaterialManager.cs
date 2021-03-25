using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public sealed class MaterialManager : USingletonObject<MaterialManager>, ISystem
	{
		public const float kUnlitLightIntensity = 0.3f;

		private static readonly Type msType = typeof(MaterialManager);

		public string systemName => msType.Name;

		public IModuleInterface module { get; set; }

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			SetGlobalLightIntensity(false);
		}
#endif

		/// <summary>
		/// 切场景之前需要还原原本的数值
		/// </summary>
		/// <param name="unlit"></param>
		public static void SetGlobalLightIntensity(bool unlit)
		{
			if (unlit)
				Shader.SetGlobalFloat(ShaderIDs.LightIntensity, kUnlitLightIntensity);
			else
				Shader.SetGlobalFloat(ShaderIDs.LightIntensity, 1.0f);
		}

		public IEnumerator Initialize()
		{
			return null;
		}

		public IEnumerator PostInitialize()
		{
			SetGlobalLightIntensity(false);
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
			SetGlobalLightIntensity(false);
		}

		public void Startup()
		{
		}

		public void Uninitialize()
		{
			SetGlobalLightIntensity(false);
		}
	}
}
