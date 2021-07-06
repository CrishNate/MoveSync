Shader "MoveSync/Projectile" 
{
    Properties 
    {
        _Color2 ("Color2", Color) = (1,1,1,1) 
        _Color ("Color", Color) = (1,1,1,1) 
        _ColorOverride ("ColorOverride", Color) = (1,1,1,1) 
        _OffsetDanger ("OffsetDanger", Range(0, 10)) = 1
    }
    
    SubShader 
    {
        Tags { "RenderType"="Unlit" }
        LOD 200

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Lambert
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _ColorOverride; 
        fixed4 _Color2; 
        fixed4 _Color;
        float _OffsetDanger; 

        struct Input
        {
            float3 viewDir;
            float4 vertex;
            float depth;
        };

		// vertex shader inputs
        struct appdata
        {
            float4 vertex   : POSITION; // vertex position
            float2 uv       : TEXCOORD0; // texture coordinate
            float3 normal   : NORMAL; // texture coordinate
        };
        
        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.depth = ComputeScreenPos(o.vertex).z;
            o.depth = max(_ProjectionParams.z * 0.3 - o.depth * _ProjectionParams.z - _OffsetDanger, 0);
        }
    
        void surf (Input IN, inout SurfaceOutput o)
        {
            half factor = pow(dot(normalize(IN.viewDir),o.Normal), 0.5f);
            
            o.Emission.rgb = _Color + (_Color2 - _Color) * factor;
            
            if (IN.depth <= 1)
            {
                fixed4 colorBlendTo = _ColorOverride * 0.75f + _ColorOverride * (1 - factor);
                o.Emission.rgb = o.Emission.rgb + (colorBlendTo - o.Emission.rgb) * (1 - IN.depth);
            }
        }
        
        ENDCG
    } 
FallBack "Unlit"
}