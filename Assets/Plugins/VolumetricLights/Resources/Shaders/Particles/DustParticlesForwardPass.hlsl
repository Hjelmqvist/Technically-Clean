#ifndef UNIVERSAL_PARTICLES_UNLIT_FORWARD_PASS_INCLUDED
#define UNIVERSAL_PARTICLES_UNLIT_FORWARD_PASS_INCLUDED

            #include "UnityCG.cginc"

            float4x4 _WorldToLocal;
            #define unity_WorldToObject _WorldToLocal

            #include "DustParticlesInput.hlsl"
            #include "../Primitives.hlsl"
            #include "../ShadowOcclusion.hlsl"


            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 wpos : TEXCOORD1;
            };

            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = v.color;
                float distSqr = dot(o.wpos - _WorldSpaceCameraPos.xyz, o.wpos - _WorldSpaceCameraPos.xyz);
                float distAtten =  saturate(_ParticleDistanceAtten / distSqr);
                o.color.a *= distAtten;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                #if VL_CUSTOM_BOUNDS
                    if (!TestBounds(i.wpos)) return 0;
                #endif

                half4 col = i.color * _ParticleTintColor * tex2D(_MainTex, i.texcoord);

                col.a *= DistanceAttenuation(i.wpos.xyz);

                col.a = saturate(col.a);

                #if VL_SHADOWS || VL_SPOT_COOKIE
                    col.rgb *= GetShadowAttenParticlesWS(i.wpos);
                #endif

                return col;
            }

#endif // UNIVERSAL_PARTICLES_UNLIT_FORWARD_PASS_INCLUDED
