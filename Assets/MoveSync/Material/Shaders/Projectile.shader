Shader "MoveSync/Projectile" 
{
    Properties 
    {
        _Color2 ("Color2", Color) = (1,1,1,1) 
        _Color ("Color", Color) = (1,1,1,1) 
        _ColorOverride ("ColorOverride", Color) = (1,1,1,1) 
    }
    
    SubShader 
    {
        Tags { "RenderType"="Unlit" }
        LOD 200

        Lighting Off
        Fog { Mode Off }
        
        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Lambert
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

         UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color2)
            UNITY_DEFINE_INSTANCED_PROP(float4, _ColorOverride)
        UNITY_INSTANCING_BUFFER_END(Props)
    
        static float _OffsetDanger = 1;
		static float _InvRangeDanger = 0.25f;

        struct Input
        {
            float3 viewDir;
            float4 vertex;
            float depth;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

		// vertex shader inputs
        struct appdata_t
        {
            float4 vertex   : POSITION; // vertex position
            float2 uv       : TEXCOORD0; // texture coordinate
            float2 texcoord1       : TEXCOORD1; // texture coordinate
            float2 texcoord2       : TEXCOORD2; // texture coordinate
            float3 normal   : NORMAL; // texture coordinate
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        void vert(inout appdata_t v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.depth = clamp((distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex)) - _OffsetDanger) * _InvRangeDanger, 0, 1);
        }
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            half factor = pow(dot(normalize(IN.viewDir),o.Normal), 0.5f);

            float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            float4 color2 = UNITY_ACCESS_INSTANCED_PROP(Props, _Color2);
            float4 colorOverride = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorOverride);
            o.Emission.rgb = color + (color2 - color) * factor;
            
            if (IN.depth <= 1)
            {
                fixed4 colorBlendTo = colorOverride * 0.75f + colorOverride * (1 - factor);
                o.Emission.rgb = o.Emission.rgb + (colorBlendTo - o.Emission.rgb) * (1 - IN.depth);
            }
        }
        
        ENDCG
    } 
FallBack "Unlit"
}