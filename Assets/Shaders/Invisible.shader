Shader "Custom/Invisible"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(float4 v : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(v);
            }

            float4 frag() : SV_Target
            {
                discard;        // kill the pixel
                return float4(0,0,0,0);   // required dummy return
            }
            ENDCG
        }
    }
}
