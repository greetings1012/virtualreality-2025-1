Shader "Custom/BoxClipShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // 표면 셰이더 설정
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4x4 _TargetWorldToLocal;
        float3 _TargetBoxHalfSize;
        float3 _TargetBoxCenter;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 localPos = mul(_TargetWorldToLocal, float4(IN.worldPos, 1.0)).xyz;
            float3 relativePos = localPos - _TargetBoxCenter;
            float3 diff = abs(relativePos) - _TargetBoxHalfSize;

            if (any(diff > 0.01F))
                clip(-1);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}