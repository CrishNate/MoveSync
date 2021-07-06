// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MoveSync/UnlitTransparentColorFogBillboard" {
    Properties {
		_Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _FogNoise ("FogNoise", 2D) = "white" {}
        _ColorOverride ("ColorOverride", Color) = (1,1,1,1) 
        _OffsetDanger ("OffsetDanger", Range(0, 10)) = 1
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
                float4 worldPos	: TEXCOORD1;
				fixed4 color    : COLOR;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
				o.color = v.color * _Color;
                o.uv = v.uv;

            	// billboard mesh towards camera
				float3 vpos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
				float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
            	float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(v.vertex.x, v.vertex.y, 0.0, 0.0) * length(vpos) * 2.0f;
				o.vertex = mul(UNITY_MATRIX_P, viewPos);

                return o;
            }

            // texture we will sample
            sampler2D _MainTex;
            sampler2D _FogNoise;
		    
            float4 frag(v2f i):COLOR
		    {
                fixed4 colFog = tex2D(_FogNoise, float2(i.worldPos.x + i.worldPos.z / 2, i.worldPos.y + i.worldPos.z / 2) * 0.01f + _Time.x);
                fixed4 colGlow = tex2D(_MainTex, i.uv);

                i.color.a *= (colGlow.a + colFog.r * colGlow.a);
            	return i.color;
            }
            ENDCG
        }
    }
}