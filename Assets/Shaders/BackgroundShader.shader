Shader "Unlit/BackgroundShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondaryTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SecondaryTex;
            float4 _MainTex_ST;
            float4 _SecondaryTex_ST;

            int _LightMeshVectorsLength = 1023;
            float4 _LightMeshVectors[1023];

            float sign(float3 p1, float4 p2, float4 p3)
            {
                return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
            }

            bool is_in_triangle(float3 pt, float4 v1, float4 v2, float4 v3)
            {
                float d1, d2, d3;
                bool has_neg, has_pos;

                d1 = sign(pt, v1, v2);
                d2 = sign(pt, v2, v3);
                d3 = sign(pt, v3, v1);

                has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

                return !(has_neg && has_pos);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                bool bright = false;
                for (int k = 0; k < _LightMeshVectorsLength; k += 3)
                {
                    float4 vec0 = _LightMeshVectors[k];
                    float4 vec1 = _LightMeshVectors[k + 1];
                    float4 vec2 = _LightMeshVectors[k + 2];
                    if (!(vec0.x == vec1.x && vec0.x == vec2.x) && is_in_triangle(i.worldPos, vec0, vec1, vec2))
                    {
                        bright = true;
                        break;
                    }
                }
                fixed4 col = tex2D(_SecondaryTex, i.uv);
                if (bright)
                {
                    // sample the texture
                    col = tex2D(_MainTex, i.uv);
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
