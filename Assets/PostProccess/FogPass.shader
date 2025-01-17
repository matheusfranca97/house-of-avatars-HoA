// Example Shader code
Shader "Custom/PostProcessingEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // Define any other properties here
    }

    SubShader
    {
        Tags { "RenderType"="Opaque"
                "RenderQueue"="Overlay" }
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture and apply the effect
                fixed4 col = tex2D(_MainTex, i.uv);
                // Apply your post-processing effect here
                // Example: col.rgb *= 2; // Double the brightness
                float depth = clamp(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv),0,1);
                float Sky = clamp(1-depth - .999,0,1)*1000;
                float fog = clamp(lerp(1,-40,depth), 0,1);
                fog = fog*fog*fog;
                float4 fogColored = fog * float4(0.39f,0.27f,0.42f,1);
                return  fogColored+col;
            }
            ENDCG
        }
    }
}
