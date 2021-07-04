Shader "MoveSync/UIFlowShader" 
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _FlowTex ("FlowTex", 2D) = "white" {}
		_ColorFlow ("TintFlow", Color) = (1,1,1,1)
		_Scale ("Scale", Range(0, 1)) = 0.25
		_SpeedX ("ScaleX", Range(0, 50)) = 10
    	
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader 
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        // First Pass
        Pass 
        {
            Name "Default"
		    CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #pragma target 2.0

		    #include "UnityUI.cginc"
            #include "UnityCG.cginc"
		    
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

		    // vertex shader inputs
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
		    
            struct v2f
		    {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
		    
			fixed4 _Color;
			fixed4 _ColorFlow;
            sampler2D _MainTex;
            sampler2D _FlowTex;
            float4 _MainTex_ST;
            fixed4 _TextureSampleAdd;
		    
            float _Scale;
            float _SpeedX;
		    
            v2f vert(appdata_t v)
		    {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
            	
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            float4 frag(v2f i) : SV_Target
		    {
                half4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd);
                half4 colFlow = tex2D(_FlowTex, i.uv * _Scale + float2(_Time.x * _SpeedX, 0));
		    	
		    	color.rgb = (_Color + (_ColorFlow - _Color) * colFlow.r * _ColorFlow.a).rgb;
		    	color *= i.color;

            	return color;
            }
            ENDCG
        }
    }
}