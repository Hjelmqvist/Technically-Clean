#ifndef PARTICLES_UNLIT_INPUT_INCLUDED
#define PARTICLES_UNLIT_INPUT_INCLUDED

#ifndef SHADER_API_PS4
CBUFFER_START(UnityPerMaterial)
#endif

float4 _SoftParticleFadeParams;
float4 _CameraFadeParams;
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _BaseColorAddSubDiff;
half _DistortionStrengthScaled;
half _DistortionBlend;

float4 _ConeTipData, _ConeAxis;
float4 _ExtraGeoData;
float3 _BoundsCenter, _BoundsExtents;
float _Border, _DistanceFallOff;
float3 _FallOff;

half4 _Color;
float4 _AreaExtents;
half4 _ParticleTintColor;
float _ParticleDistanceAtten;

#ifndef SHADER_API_PS4
CBUFFER_END
#endif

#endif // PARTICLES_UNLIT_INPUT_INCLUDED
