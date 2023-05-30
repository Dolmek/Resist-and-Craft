Shader "Custom/FogOfWar" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _DepthTex("Depth Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (0, 0, 0, 1)
        _FogDistance("Fog Distance", Range(0, 1)) = 0.5
    }
        SubShader{
            Pass {
                Tags {"LightMode" = "ForwardBase"}

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
                    float4 vertex : SV_POSITION;
                    float4 pos : TEXCOORD1;
                };

                sampler2D _MainTex;
                sampler2D _DepthTex;
                float4 _FogColor;
                float _FogDistance;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.pos = ComputeGrabScreenPos(o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    // Get the depth value for the current pixel
                    float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_DepthTex, i.pos));

                // Calculate the fog amount based on the distance from the camera
                float fogAmount = (depth - _FogDistance) * 2.0;

                // Apply the fog amount to the color
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, _FogColor.rgb, fogAmount);
                return col;
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}