Shader "Loki/Lines"
{
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Cull Off
			Fog { Mode Off }

			BindChannels
			{
				  Bind "vertex", vertex
				  Bind "color", color
			}
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual
			Cull off
			Fog { Mode Off }

			BindChannels
			{
				  Bind "vertex", vertex
				  Bind "color", color
			}
		}
	}
}