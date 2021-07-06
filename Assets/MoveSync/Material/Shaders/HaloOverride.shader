Shader "MoveSync/BeatObject" 
{
    Properties 
    {
        _Color2 ("Color2", Color) = (1,1,1,1) 
        _Color ("Color", Color) = (1,1,1,1) 
        _ColorOverride ("ColorOverride", Color) = (1,1,1,1) 
        _Override ("Override", Range (0, 1)) = 1
    }
    
    SubShader 
    {
        Tags { "RenderType"="Unlit" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed4 _ColorOverride; 
        fixed4 _Color2; 
        fixed4 _Color;
        float _Override;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            half factor = pow(dot(normalize(IN.viewDir),o.Normal), 0.5f);
            
            o.Emission.rgb = _Color + (_Color2 - _Color) * factor;
            
            if (_Override > 0)
            {
                fixed4 colorBlendTo = _ColorOverride * 0.75f + _ColorOverride * (1 - factor);
                o.Emission.rgb = o.Emission.rgb + (colorBlendTo - o.Emission.rgb) * _Override;
            }
        }
        ENDCG
    } 
FallBack "Unlit"
}