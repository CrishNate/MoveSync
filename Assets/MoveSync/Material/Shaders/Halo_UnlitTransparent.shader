// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MoveSync/Halo_UnlitTransparent" {
Properties {
        _Shininess ("Shininess", Range (0.01, 3)) = 1
        _Color ("Color", Color) = (1,1,1,1) 
        _Fill ("Fill", Range (0, 1)) = 1
}
 
SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
   
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    
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
            half factor = dot(normalize(IN.viewDir), o.Normal);
            o.Emission.rgb = _Color * _Fill + _Color * (_Shininess - factor * _Shininess);
            o.Alpha = c.a;
        }
    ENDCG
    
    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
           
            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
           
            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                return col;
            }
        ENDCG
    }
}
 
}