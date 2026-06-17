Shader "UI/RoundedRect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        // Raggio degli angoli (0..0.5 in unità UV: 0.5 = cerchio completo)
        _Radius ("Corner Radius (0..0.5)", Range(0,0.5)) = 0.15

        // Bordo opzionale
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width (UV units)", Range(0,0.2)) = 0.0

        // Morbidezza bordo (anti-alias). 0 = netto, 1 = morbido
        _Softness ("Edge Softness", Range(0,1)) = 0.02

        // UI / Stencil compat (bene con Button/Image standard)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Radius;         // 0..0.5 (in UV)
            float _OutlineWidth;   // banda di bordo in UV
            float _Softness;       // AA in UV
            fixed4 _OutlineColor;

            // UI default vertex
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0; // useremo i UV come spazio 0..1 del rettangolo
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); // 0..1
                o.color = v.color * _Color;
                return o;
            }

            // Signed Distance di un rounded-rect centrato in 0.5,0.5 con raggio _Radius (in UV)
            float sdRoundedRect(float2 uv, float radius)
            {
                // porta uv in spazio centrato
                float2 p = abs(uv - 0.5) - (0.5 - radius);
                float2 q = max(p, 0.0);
                return length(q) - radius; // sd < 0 = dentro; sd > 0 = fuori
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // SDF del rettangolo arrotondato
                float sd = sdRoundedRect(i.uv, _Radius);

                // Antialias: usa fwidth per morbidezza coerente a schermo
                float aa = max(_Softness, fwidth(sd));

                // Riempimento (dentro = alpha 1)
                float fillAlpha = saturate(1.0 - smoothstep(0.0, aa, sd));

                // Bordo opzionale: banda quando sd è vicino a 0 (spessore _OutlineWidth)
                float borderAlpha = 0.0;
                if (_OutlineWidth > 0.0)
                {
                    // Attivo nella fascia |sd| < _OutlineWidth
                    float band = 1.0 - smoothstep(_OutlineWidth, _OutlineWidth + aa, abs(sd));
                    borderAlpha = saturate(band);
                }

                // Colori: il bordo "disegna" sopra al fill solo vicino all’edge
                fixed4 col = i.color;
                fixed4 outCol = col;
                // Se c’è bordo, miscelalo sul fill in prossimità dell’edge
                if (_OutlineWidth > 0.0)
                {
                    outCol.rgb = lerp(outCol.rgb, _OutlineColor.rgb, borderAlpha);
                    outCol.a   = max(fillAlpha, borderAlpha) * col.a;
                }
                else
                {
                    outCol.a = fillAlpha * col.a;
                }

                // opzionale: campiona _MainTex se ti serve un pattern/gradiente: 
                // fixed4 texCol = tex2D(_MainTex, i.uv);
                // outCol.rgb *= texCol.rgb; outCol.a *= texCol.a;

                return outCol;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
