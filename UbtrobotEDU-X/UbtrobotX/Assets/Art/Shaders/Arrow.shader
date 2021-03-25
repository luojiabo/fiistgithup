Shader "Ubtrobot/Arrow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Diffuse("Diffuse",Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss", Range(5,30)) = 10 
	}
	SubShader
	{
		Pass
		{
			Tags{ 
				"LightMode" = "ForwardBase" 
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			ZWrite off
			ZTest off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float2 uv : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld,v.normal));
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse =  _Diffuse.rgb * saturate(dot(i.worldNormal,worldLightDir));
				fixed4 col = tex2D(_MainTex, i.uv) * fixed4(diffuse + _Specular.rgb,1.0);
				return col;
			}
			ENDCG
		}
	}
	Fallback "Specular"
}