Shader "Unlit/PlayerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _OutlineDistance ("Outline Distance (px)", Range(0, 100)) = 2
        _OutlineThickness ("Outline Thickness (px)", Range(0, 100)) = 1
        _OutlineColor ("Outline Color (RGBA)", Color) = (0,0,0,1)
        _OutlinePower ("Outline Power", Range(0, 10)) = 0.5
        _AlphaCutoff ("Opaque Alpha Cut", Range(0,1)) = 0.99
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize; // xy = 1/width,1/height

            float _OutlineDistance;
            float _OutlineThickness;
            float _OutlinePower;
            fixed4 _OutlineColor;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f,o);
                UNITY_TRANSFER_INSTANCE_ID(v,o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // 12 directions sur le cercle (constantes → coût fixe)
            static const float2 DIRS[12] = {
                float2( 1, 0), float2( 0.5,  0.8660254), float2(-0.5,  0.8660254),
                float2(-1, 0), float2(-0.5, -0.8660254), float2( 0.5, -0.8660254),
                float2( 0.9238795,  0.3826834), float2( 0.3826834,  0.9238795),
                float2(-0.3826834,  0.9238795), float2(-0.9238795,  0.3826834),
                float2(-0.9238795, -0.3826834), float2( 0.3826834, -0.9238795)
            };

            // Dilation binaire: max(step(alpha - cutoff)) sur un anneau de rayon r
            float DilateAtRadius(float2 uv, float rPx)
            {
                // Convertit rayon en UV via taille texel
                float2 stepUV = float2(_MainTex_TexelSize.x * rPx, _MainTex_TexelSize.y * rPx);

                // Échantillons fixes (12) + 12 supplémentaires à un rayon légèrement jitteré
                // pour limiter les trous (24 taps au total).
                float acc = 0.0;

                [unroll]
                for (int k = 0; k < 12; ++k)
                {
                    float2 o1 = uv + DIRS[k] * stepUV;
                    float a1 = tex2D(_MainTex, o1).a;
                    acc = max(acc, step(_AlphaCutoff, a1));

                    // second anneau très proche (r * 0.7) pour combler les gaps directionnels
                    float2 o2 = uv + DIRS[k] * (stepUV * 0.7);
                    float a2 = tex2D(_MainTex, o2).a;
                    acc = max(acc, step(_AlphaCutoff, a2));
                }
                return saturate(acc);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Si le pixel est déjà opaque, pas d’outline à ajouter par-dessus.
                // (On peut vouloir un halo sur les bords internes: retirer ce early-out si nécessaire.)
                if (col.a >= _AlphaCutoff)
                {
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }

                // Rayons en pixels
                float innerPx = max(0.0, _OutlineDistance);
                float outerPx = max(innerPx, innerPx + _OutlineThickness);

                // Dilation binaire aux deux rayons, puis différence -> bande d’outline
                float dInner = DilateAtRadius(i.uv, innerPx);
                float dOuter = DilateAtRadius(i.uv, outerPx);

                // Masque de l’anneau (outer ET NON inner)
                float ring = saturate(dOuter - dInner);

                // Lissage optionnel: adoucit légèrement le bord externe avec une mip-cover
                // (pas indispensable; faible coût et visuel plus propre)
                // float2 ddxUV = ddx(i.uv) * outerPx;
                // float2 ddyUV = ddy(i.uv) * outerPx;

                fixed4 outCol = col;
                if (ring > 0.0)
                {
                    // Additif contrôlé par _OutlinePower et alpha de la couleur
                    float w = ring * _OutlinePower * _OutlineColor.a;
                    outCol.rgb = outCol.rgb + w * _OutlineColor.rgb;
                    outCol.a   = saturate(outCol.a + ring * _OutlineColor.a);
                }

                UNITY_APPLY_FOG(i.fogCoord, outCol);
                return outCol;
            }
            ENDCG
        }
    }
}
