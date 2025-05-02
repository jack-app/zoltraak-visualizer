Shader "Custom/circle"
{
    Properties
    {
        _MainTex ("Main Texture (RGBA)", 2D) = "white" {}
        _EmissionTex ("Emission Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Blending for transparency
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _EmissionTex;
            fixed4 _EmissionColor;
            float _AlphaCutoff;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Discard fully transparent (or mostly transparent) pixels
                if (col.a < _AlphaCutoff)
                    discard;

                // Get emission only for visible areas
                fixed4 emission = tex2D(_EmissionTex, i.uv) * _EmissionColor;

                // Final output: base color + emission
                fixed4 finalColor = col + emission;
                finalColor.a = col.a; // Preserve original alpha for blending
                return finalColor;
            }
            ENDCG
        }
    }
}