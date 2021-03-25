Shader "Ubtrobot/ColorTex" {
    Properties {
        _MainTex ("MainTex ", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {

			Blend SrcAlpha OneMinusSrcAlpha
			// ZTest on
            Offset -1, 1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex; 
			float4 _MainTex_ST;
            float4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            v2f vert (appdata v) {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(v2f i) : COLOR {
                float4 col = tex2D(_MainTex, i.uv0);
                float3 finalColor = (_Color.rgb * col.rgb);
                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
