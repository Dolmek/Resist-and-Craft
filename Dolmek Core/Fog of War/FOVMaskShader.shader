Shader "Custom/FOVMaskShader" {
    SubShader{
        Pass {
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            void vert(inout appdata v) {
                v.vertex = UnityObjectToClipPos(v.vertex);
            }

            void frag(out float4 color : SV_Target, in float depth : SV_Depth) {
                color = float4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}