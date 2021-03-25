using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ubtrobot
{
	public class ShaderIDs
	{
		public static readonly int Gloss;
		public static readonly int MainTex;
		public static readonly int Diffuse;
		public static readonly int Specular;
		public static readonly int LightIntensity;
		public static readonly int HighlightIntensity;
		public static readonly int OutlineColor;
		public static readonly int OutlineLength;

		static ShaderIDs()
		{
			Gloss = Shader.PropertyToID("_Gloss");
			MainTex = Shader.PropertyToID("_MainTex");
			Diffuse = Shader.PropertyToID("_Diffuse");
			LightIntensity = Shader.PropertyToID("_LightIntensity");
			HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
			OutlineColor = Shader.PropertyToID("_OutlineColor");
			OutlineLength = Shader.PropertyToID("_Outline");
		}
	}
}
