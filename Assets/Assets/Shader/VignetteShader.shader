Shader "Custom/UIVignette"
{
    Properties
    {
        _Intensity ("Intensity", Range(0, 2)) = 1
        _Power ("Falloff", Range(0.1, 4)) = 0.25
        _Color ("Vignette Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Intensity;
            float _Power;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Generate vignette mask
                uv *= (1.0 - uv.yx);

                float vig = uv.x * uv.y * 15.0;

                vig = pow(vig, _Power);

                // Convert to alpha mask
                float alpha = saturate((1.0 - vig) * _Intensity);

                // vignette
                return float4(_Color.rgb, alpha * _Color.a);
            }

            ENDCG
        }
    }
}