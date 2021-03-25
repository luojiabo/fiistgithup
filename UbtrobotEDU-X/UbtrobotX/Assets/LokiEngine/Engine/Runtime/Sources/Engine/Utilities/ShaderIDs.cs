using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Loki
{
	public static class ShaderIDs
	{
		public static readonly int MainTex;
		public static readonly int TintColor;

		static ShaderIDs()
		{
			MainTex = Shader.PropertyToID("_MainTex");
			TintColor = Shader.PropertyToID("_TintColor");
		}
	}
}
