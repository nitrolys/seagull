// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestToon"
{
Properties
    {
        // we have removed support for texture tiling/offset,
        // so make them not be displayed in material inspector
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
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
            sampler2D _MainTex;

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture and return it
                fixed4 color = tex2D(_MainTex, i.uv);

                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 N = i.worldNormal;

                if (dot(V, N) < 0.2) {
                    color = float4(0, 0, 0, 1);
                }

                return color;
            }
            ENDCG
        }
    }
}
