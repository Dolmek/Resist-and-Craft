Shader "Custom/FOV" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _FOV("Field of View", Range(0.0, 180.0)) = 60.0
    }
        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 worldPos : TEXCOORD1;
                    UNITY_FOG_COORDS(2)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _FOV;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                half4 frag(v2f input) : SV_Target{
                    half4 col = tex2D(_MainTex, input.uv);
                    half dist = distance(input.worldPos, _WorldSpaceCameraPos);
                    col.a *= step(dist, _FOV);
                    col.a *= 1.0 - smoothstep(_FOV - 2.0, _FOV, dist);
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}