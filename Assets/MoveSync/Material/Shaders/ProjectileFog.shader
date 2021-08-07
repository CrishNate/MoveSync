// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MoveSync/ProjectileFog" {
    Properties 
	{
		_Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _FogNoise ("FogNoise", 2D) = "white" {}
    }
    
    SubShader 
	{
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Additive"}
        
        ZWrite Off
        Lighting Off
        Cull Off
        Fog { Mode Off }

        Blend SrcAlpha One
    	
        // First Pass
        Pass 
    	{
		    CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
			#pragma multi_compile_instancing
            #include "UnityCG.cginc"
		    
			UNITY_INSTANCING_BUFFER_START(Props)
	            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)
    
	        static float _OffsetDanger = 1;
			static float _InvRangeDanger = 0.25f;

		    // vertex shader inputs
            struct appdata
            {
                float4 vertex   : POSITION; // vertex position
                float2 uv       : TEXCOORD0; // texture coordinate
				fixed4 color    : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
		    
            struct v2f
		    {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float depth		: TEXCOORD1;
                float4 worldPos	: TEXCOORD2;
				fixed4 color    : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
		    {
                v2f o;
            	
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
            	
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
				o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                o.uv = v.uv;

				// billboard
				float3 vpos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
				float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
            	float4 viewPos =  mul(UNITY_MATRIX_V, worldCoord) + float4(v.vertex.x, v.vertex.y, 0.0, 0.0) * length(vpos) * 2.0f;
				o.vertex = mul(UNITY_MATRIX_P, viewPos);
				
				o.depth = clamp((distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex)) - _OffsetDanger) * _InvRangeDanger, 0, 1);
            	o.color.a *= o.depth;
            	
                return o;
            }

            // texture we will sample
            sampler2D _MainTex;
            sampler2D _FogNoise;
		    
            float4 frag(v2f i):COLOR
		    {
		    	UNITY_SETUP_INSTANCE_ID(i);
                fixed4 colFog = tex2D(_FogNoise, float2(i.worldPos.x + i.worldPos.z / 2, i.worldPos.y + i.worldPos.z / 2) * 0.01f + _Time.x);
                fixed4 colGlow = tex2D(_MainTex, i.uv);

                i.color.a *= (colGlow.a + colFog.r * colGlow.a);
            	return i.color;
            }
            ENDCG
        }
    }
}