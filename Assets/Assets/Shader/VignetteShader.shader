// Creates a configurable vignette effect for UI elements and screen overlays.
Shader "Custom/UIVignette"
{
    Properties
    {
        // Controls the overall strength of the vignette effect.
        _Intensity ("Intensity", Range(0, 2)) = 1

        // Controls how far the vignette fades towards the centre.
        _Falloff ("Falloff", Range(0, 4)) = 0.25

        // Sets the colour of the vignette.
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

            // Overall strength of the vignette.
            float _Intensity;

            // Controls the size and softness of the vignette.
            float _Falloff;

            // Colour and transparency of the vignette.
            fixed4 _Color;

            // Input data passed into the vertex shader.
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Data passed from the vertex shader to the fragment shader.
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Converts vertices to clip space and passes UV coordinates through.
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Calculates the vignette colour and transparency for each pixel.
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Generate a radial mask based on the UV coordinates.
                uv *= (1.0 - uv.yx);

                // Calculate the vignette intensity.
                float vig = uv.x * uv.y * 15.0;

                // Apply the configurable falloff.
                vig = pow(vig, _Falloff);

                // Convert the vignette into an alpha value.
                float alpha = saturate((1.0 - vig) * _Intensity);

                // Return the final vignette colour and transparency.
                return float4(_Color.rgb, alpha * _Color.a);
            }

            ENDCG
        }
    }
}