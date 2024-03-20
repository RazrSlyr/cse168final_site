Shader "Custom/VertexColorTransparencyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float4 vertexColor : COLOR;
        };

        fixed4 _Color;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexColor = v.color;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = _Color * IN.vertexColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a * _Color.a; // Multiply by the alpha of the main color
        }
        ENDCG
    }
    FallBack "Diffuse"
}
