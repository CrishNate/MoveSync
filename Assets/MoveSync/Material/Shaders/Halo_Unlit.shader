Shader "MoveSync/Halo_Unlit" {
    Properties {
        _Shininess ("Shininess", Range (0.01, 3)) = 1
        _Color ("Color", Color) = (1,1,1,1) 
        _Fill ("Fill", Range (0, 1)) = 1
        

    }
    SubShader {
        Tags { "RenderType"="Unlit" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Shininess;
        float _Fill;
        fixed4 _Color; 

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half factor = dot(normalize(IN.viewDir),o.Normal);
            o.Emission.rgb = _Color * _Fill + _Color * (_Shininess - factor * _Shininess);
            o.Alpha = c.a;
        }
        ENDCG
    } 
FallBack "Unlit"
}