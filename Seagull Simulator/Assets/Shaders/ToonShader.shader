// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToonShader"
{
Properties
    {
        // we have removed support for texture tiling/offset,
        // so make them not be displayed in material inspector
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _r_begin_transparent ("Distance to begin transparency", Float) = 2
        _r_most_transparent ("Distance for max transparency", Float) = 0.5
        _max_transparent ("Max alpha transparency", Range (0, 1)) = 0.2
        _tint ("Tint Color", Color) = (1, 1, 1, 1)
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
            uniform fixed4 _tint;

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture and return it
                fixed4 color = tex2D(_MainTex, i.uv);

                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 N = i.worldNormal;
                float3 L = _WorldSpaceLightPos0.xyz;  // Assumes directional light
                float r = distance(_WorldSpaceCameraPos, i.worldPos);

                // Diffuse lighting
                float diffuseShade = max(dot(N, L), 0);
                float ambientShade = 0.1;

                float discreteShade = ceil((diffuseShade + ambientShade) * 1.75) / 2;
                color = discreteShade * color;

                color = color * _tint;

                if (r < _r_begin_transparent) {
                    if (r < _r_most_transparent) {
                        color.w = _max_transparent;
                    }else{
                        color.w = 1 - (_r_begin_transparent - r) * (1 - _max_transparent) / (_r_begin_transparent - _r_most_transparent);
                    }
                }else{
                    color.w = 1;
                }

                return color;
            }
            ENDCG
        }

        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
