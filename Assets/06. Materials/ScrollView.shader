Shader "Custom/ScrollView"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _AlbedoColor ("Albedo Color", Color) = (1,0.5,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }

        Pass
        {
            Name "SCROLLVIEW"
            Cull Front
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _AlbedoColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            // UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); //Insert

            fixed4 frag(v2f i) : SV_Target
            {
                return _AlbedoColor;
            }

            ENDCG
        }
    }
}