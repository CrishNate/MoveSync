Shader "MoveSync/WorldOutline" {
    Properties {
        _ColorOut ("ColorOut", Color) = (1,1,1,1) 
        _ColorIn ("ColorIn", Color) = (1,1,1,1) 
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
        fixed4 _ColorOut; 
        fixed4 _ColorIn;
        float _Offset;
        float _Factor;

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half factor1 = dot(normalize(IN.viewDir), normalize(IN.worldNormal));
            half factor2 = dot(normalize(IN.worldRefl), normalize(IN.viewDir));
            half factor = (factor1 * (1 - factor2));
            factor *= 2;
            factor -= 1;
            factor = 1 / (1 + exp(-factor * _Factor * abs(_Offset - 0.5)));
            
            fixed4 color1 = _ColorOut + (_ColorIn - _ColorOut) * _Offset;
            fixed4 color2 = _ColorIn + (_ColorOut - _ColorIn) * _Offset;
            
            o.Emission.rgb = color1 + (color2 - color1) * factor;
        }
        ENDCG
    } 
FallBack "Unlit"
}