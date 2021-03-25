// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Loki/Unlit/UV-Based Wireframe"
{
	Properties
	{
		_LineColor("Line Color", Color) = (1, 1, 1, 1)
		_GridColor("Grid Color", Color) = (0, 0, 0, 1)
		_LineWidth("Line Width", float) = 0.05
	}

		SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _LineColor;
			uniform float4 _GridColor;
			uniform float _LineWidth;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float4 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float2 uv = i.uv;

				if (uv.x < _LineWidth)
					return _LineColor;
				else if (uv.x > 1 - _LineWidth)
					return _LineColor;
				else if (uv.y < _LineWidth)
					return _LineColor;
				else if (uv.y > 1 - _LineWidth)
					return _LineColor;
				else if (uv.x - uv.y < _LineWidth || uv.x - uv.y > -_LineWidth)
					return _LineColor;
				else
					return _GridColor;
			}
			ENDCG
		}
	}
}
