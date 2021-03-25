// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ubtrobot/Outline" {

	Properties {
	
		_Outline ("Outline Length", Range(0.0, 1.0)) = 0.02
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1.0)
		[Toggle(VIEWSPACE_OUTLINE)]_ViewSpace("Enable ViewSpace Outline", float) = 1 
		[Stencil]_OutlineStencil("Stencil Ref", float) = 128
	}
	
	SubShader {
	
		Tags { 
			"RenderType"="Opaque"
			"Queue"="Transparent"
		}
	
		LOD 200
	
		// render model
		Pass {
		    name "HideCrossEdge"
			ColorMask 0
            // ZWrite off
            Cull back
            Stencil {
                Ref [_OutlineStencil]
                Comp Always
                Pass replace
                ZFail replace
            }
		}

		// render outline
		Pass {
		    name "Outline"
		    
			Stencil {
				Ref [_OutlineStencil]
				Comp NotEqual
			}

			Cull back
			ZWrite Off
			
			Blend SrcAlpha OneMinusSrcAlpha 
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature __ VIEWSPACE_OUTLINE
			#include "UnityCG.cginc"
			
			fixed _Outline;
			float4 _OutlineColor;
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
			};
			
			v2f vert(appdata v) {
				v2f o;
				float4 vert = v.vertex;
				#if VIEWSPACE_OUTLINE
				fixed3 norm = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				norm = normalize(norm);
                norm.x *= UNITY_MATRIX_P[0][0];
                norm.y *= UNITY_MATRIX_P[1][1];
				o.pos = UnityObjectToClipPos(vert);
				o.pos.xy += norm.xy * _Outline;
				#else
				vert.xyz += v.normal * _Outline;
				o.pos = UnityObjectToClipPos(vert);
                #endif
				return o;
			}
			
			half4 frag(v2f i) : COLOR {
				return _OutlineColor;
			}	
			
			ENDCG
		}

	} 
	
	FallBack "Diffuse"
}