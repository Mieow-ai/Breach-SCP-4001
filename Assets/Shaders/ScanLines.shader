Shader "UI/ScanLines_Transparent"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (0,0,0,1) // Noir par défaut
        _BackgroundColor ("Background Color", Color) = (0,0,0,0) // Transparent
        _Density ("Density", Float) = 200
        _Thickness ("Thickness", Range(0,1)) = 0.02
    }

    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 vertex : SV_POSITION;
            };

            fixed4 _LineColor;
            fixed4 _BackgroundColor;
            float _Density;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Génère des lignes horizontales
                float scanLine = step(frac(i.uv.y * _Density), _Thickness);
                
                // Mixe entre la couleur de ligne et le fond transparent
                return lerp(_BackgroundColor, _LineColor, scanLine);
            }
            ENDCG
        }
    }
}
