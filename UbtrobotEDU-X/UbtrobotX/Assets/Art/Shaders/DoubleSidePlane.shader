Shader "Ubtrobot/DoubleSidePlane"
{
	Properties
	{
		_Diffuse("Diffuse", Color) = (1,1,1,1)
	}
	SubShader
	{
		Pass
		{
			Tags{ 
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			Blend SrcAlpha OneMinusSrcAlpha

			Cull off
			ZWrite off
			ZTest off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Diffuse;

			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			fixed4 frag() : SV_Target
			{
				return _Diffuse;
			}
			ENDCG
		}
	}
}