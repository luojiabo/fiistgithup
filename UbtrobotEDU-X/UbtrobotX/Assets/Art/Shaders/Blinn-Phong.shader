Shader "Ubtrobot/Blinn-Phong"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Diffuse("Diffuse",Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss", Range(5,30)) = 10 
		_HighlightIntensity("Highlight Intensity",Range(0, 1)) = 0
		[Toggle(ENABLE_HIGHLIGHT)]_EnableHighLight("EnableHighlight",float) = 1.0
		[Toggle(DISABLE_HIGHLIGHT)]_DisableHighLight("DisableHighlight",float) = 0.0
	}

	SubShader
	{
		Pass
		{
			Tags{ 
				"LightMode" = "ForwardBase" 
				"Queue"="Geometry"
				"RenderType"="Opaque"
			}
			
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_local ENABLE_HIGHLIGHT DISABLE_HIGHLIGHT

			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			uniform fixed _LightIntensity;
			sampler2D _MainTex;
			fixed4 _Diffuse;
			fixed4 _Specular;
			fixed4 _HightlightColor;
			float _Gloss;
			float4 _MainTex_ST;
			fixed _HighlightIntensity;

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

				o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld,v.normal));   //法向量从对象坐标系转换为世界坐标
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;       //顶点从对象坐标系转换为世界坐标

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);

				//unlit
				//fixed4 col = tex2D(_MainTex, i.uv) * _Specular;
				//return col;

				//Blinn-phong
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed4 lightColor = _LightColor0;
				#if ENABLE_HIGHLIGHT
				lightColor = (_LightIntensity + _HighlightIntensity) * lightColor;
				#endif

				fixed3 diffuse = lightColor.rgb * _Diffuse.rgb * saturate(dot(i.worldNormal,worldLightDir));

				fixed3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				fixed3 halfDir = normalize(worldLightDir + ViewDir);

				fixed3 specular = lightColor.rgb* _Specular.rgb * pow(saturate(dot(ViewDir,halfDir)),_Gloss);
				return tex2D(_MainTex, i.uv) * fixed4(diffuse + specular + ambient,1.0);
			}
			ENDCG
		}
	}
	Fallback "Specular"
}