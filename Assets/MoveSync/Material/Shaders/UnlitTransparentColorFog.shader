// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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
        Cull Off
        Fog { Mode Off }

        Blend SrcAlpha One
    	
        // First Pass
        Pass {
		    CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #include "UnityCG.cginc"
		    
			fixed4 _Color;
			static float _DecreaseDistance = 32;

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
                float depth		: TEXCOORD1;
                float4 worldPos	: TEXCOORD2;
				fixed4 color    : COLOR;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.depth = ComputeScreenPos(o.vertex).z;
				o.color = v.color * _Color;
                o.uv = v.uv;

                return o;
            }

            // texture we will sample
            sampler2D _MainTex;
            sampler2D _FogNoise;
		    
            float4 frag(v2f i):COLOR
		    {
                fixed4 colFog = tex2D(_FogNoise, float2(i.worldPos.x + i.worldPos.z / 2, i.worldPos.y + i.worldPos.z / 2) * 0.01f + _Time.x * 1.0f);
                fixed4 colGlow = tex2D(_MainTex, i.uv);

            	// 0.3 is somehow a magic coeficient for this depth
		    	i.depth = (_ProjectionParams.z * 0.01f - i.depth * _ProjectionParams.z) * _DecreaseDistance;
                i.color.a *= (colGlow.a + colFog.r * colGlow.a) * clamp(i.depth, 0, 1);
            	return i.color;
            }
            ENDCG
        }
    }
}