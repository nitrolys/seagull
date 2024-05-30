// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToonScript"
{
Properties
    {
        // we have removed support for texture tiling/offset,
        // so make them not be displayed in material inspector
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _r_begin_transparent ("Distance to begin transparency", Float) = 2
        _r_most_transparent ("Distance for max transparency", Float) = 0.5
        _max_transparent ("Max alpha transparency", Range (0, 1)) = 0.2
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "RanderType" = "transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
                float3 normal: NORMAL;
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 clipPos : SV_POSITION; // clip space position
                float3 worldNormal : NORMAL;
                float3 worldPos : TEXCOORD1;
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // transform position to clip space
                // (multiply with model*view*projection matrix)
                o.clipPos = UnityObjectToClipPos(v.vertex);
                // just pass the texture coordinate
                o.uv = v.uv;

                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

                // World position
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            // texture we will sample
            uniform sampler2D _MainTex;
            uniform float _r_begin_transparent;
            uniform float _r_most_transparent;
            uniform float _max_transparent;

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture and return it
                fixed4 color = tex2D(_MainTex, i.uv);

                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 N = i.worldNormal;
                float r = distance(_WorldSpaceCameraPos, i.worldPos);

                if (dot(V, N) < 0.2) {
                    color = float4(0, 0, 0, 1);
                }

                if (r < _r_begin_transparent) {
                    if (r < _r_most_transparent) {
                        color.w = _max_transparent;
                    }else{
                        color.w = 1 - (_r_begin_transparent - r) * (1 - _max_transparent) / (_r_begin_transparent - _r_most_transparent);
                    }
                }

                return color;
            }
            ENDCG
        }
    }
}
