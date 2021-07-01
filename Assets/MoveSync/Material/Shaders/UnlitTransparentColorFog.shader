// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MoveSync/UnlitTransparentColorFog" {
    Properties {
		_Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _FogNoise ("FogNoise", 2D) = "white" {}
    }
    
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Additive"}
        
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha
        
        // First Pass
        Pass {
		    CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"
		    
            // vertex shader inputs
            struct appdata
            {
                float4 vertex   : POSITION; // vertex position
                float2 uv       : TEXCOORD0; // texture coordinate
				fixed4 color    : COLOR;
            };
		    
            struct v2f {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 scrPos   : TEXCOORD1;
				fixed4 color    : COLOR;
            };
                
			fixed4 _Color;
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
				o.color = v.color * _Color;
                o.uv = v.uv;
                return o;
            }

            // texture we will sample
            sampler2D _MainTex;
            sampler2D _FogNoise;
		    
            float4 frag(v2f i):COLOR {
                const float ratio = _ScreenParams.x / _ScreenParams.y;
                fixed4 colFog = tex2D(_FogNoise, float4(i.scrPos.x / i.scrPos.w, i.scrPos.y / ratio / i.scrPos.w, 0, 0));
                fixed4 colGlow = tex2D(_MainTex, i.uv);

                i.color.a = (colGlow.a + colFog.a * colGlow.a);
                return i.color;
            }
            ENDCG
        }
    }
}