Shader "Custom/EdgeOnly"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        _Thickness ("Line Thickness", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry" }
        Pass
        {
            // IMPORTANT: enables geometry shader
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0   // GS required

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2g
            {
                float4 pos : POSITION;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _EdgeColor;
            float _Thickness;

            // Vertex shader
            v2g vert (appdata v)
            {
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // Geometry shader: expand triangle → 3 lines
            [maxvertexcount(6)]
            void geom(triangle v2g IN[3], inout LineStream<g2f> stream)
            {
                for (int i = 0; i < 3; i++)
                {
                    g2f p1;
                    g2f p2;

                    p1.pos = IN[i].pos;
                    p2.pos = IN[(i + 1) % 3].pos;

                    p1.color = _EdgeColor;
                    p2.color = _EdgeColor;

                    stream.Append(p1);
                    stream.Append(p2);
                }
            }

            // Fragment shader draws line color
            float4 frag(g2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }
}
