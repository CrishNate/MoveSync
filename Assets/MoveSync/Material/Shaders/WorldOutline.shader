Shader "MoveSync/WorldOutline" {
    Properties {
        _Color2 ("Color2", Color) = (1,1,1,1) 
        _Color ("Color", Color) = (1,1,1,1) 
        _Offset ("Offset", Range (0, 1)) = 1
        _Factor ("Factor", Range (0, 100)) = 1
    }
    
    SubShader {
        Tags { "RenderType"="Unlit" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Shininess;
        fixed4 _Color2; 
        fixed4 _Color;
        float _Offset;
        float _Factor;

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half factor1 = dot(normalize(IN.viewDir),o.Normal);
            half factor2 = dot(normalize(IN.worldRefl), normalize(IN.viewDir));
            half factor = (factor1 * (1 - factor2));
            factor *= 2;
            factor -= 1;
            factor = 1 / (1 + exp(-factor * _Factor * abs(_Offset - 0.5)));
            
            fixed4 color1 = _Color2 + (_Color - _Color2) * _Offset;
            fixed4 color2 = _Color + (_Color2 - _Color) * _Offset;
            
            o.Emission.rgb = color1 + (color2 - color1) * factor;
        }
        ENDCG
    } 
FallBack "Unlit"
}