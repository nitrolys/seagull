Shader "Unlit/EdgeFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float gradient(sampler2D tex, float2 texCoord, float2 texelSize) {
                float2 uv[2][2];
                uv[0][0] = texCoord;
                uv[1][0] = texCoord + 2 * float2(texelSize.x, 0);
                uv[0][1] = texCoord + 2 * float2(0, texelSize.y);
                uv[1][1] = texCoord + 2 * texelSize;

                float4 grad_x = tex2D(tex, uv[0][0]) - tex2D(tex, uv[1][1]);
                float4 grad_y = tex2D(tex, uv[1][0]) - tex2D(tex, uv[0][1]);

                return sqrt(dot(grad_x, grad_x) + dot(grad_y, grad_y));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 color = tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy);
                

                if (gradient(_MainTex, i.uv, _MainTex_TexelSize.xy) > 0.3)
                    return float4(0, 0, 0, 1);

                return color;
                //return color - gradient(_MainTex, i.uv, _MainTex_TexelSize.xy);
            }
            ENDCG
        }
    }
}
