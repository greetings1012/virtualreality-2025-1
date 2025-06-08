Shader "Unlit/ClipShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4x4 _TargetWorldToLocal;
            float3 _TargetBoxHalfSize;
            float3 _TargetBoxCenter;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0F)).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 localPos = mul(_TargetWorldToLocal, float4(i.worldPos, 1.0F)).xyz;
                float3 relativePos = localPos - _TargetBoxCenter;
                float3 diff = abs(relativePos) - _TargetBoxHalfSize;

                if (any(diff.xy > 0.0001F))
                {
                    clip(-1);
                }
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb *= 0.7F;

                col.a = 1.0F;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
