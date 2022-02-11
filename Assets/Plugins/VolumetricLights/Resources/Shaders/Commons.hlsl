#ifndef VOLUMETRIC_LIGHTS_COMMONS
#define VOLUMETRIC_LIGHTS_COMMONS

#include "UnityCG.cginc"
#include "Options.hlsl"

UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

#ifndef SHADER_API_PS4
CBUFFER_START(UnityPerMaterial)
#endif

float4 _ConeTipData, _ConeAxis;
float4 _ExtraGeoData;
float3 _BoundsCenter, _BoundsExtents;
float4 _ToLightDir;

float jitter;
float _NoiseScale, _NoiseStrength, _NoiseFinalMultiplier, _Border, _DistanceFallOff;
float3 _FallOff;
half4 _Color;
float4 _AreaExtents;

float4 _RayMarchSettings;
float4 _WindDirection;
half4 _LightColor;
half  _Density;

#ifndef SHADER_API_PS4
CBUFFER_END
#endif

sampler3D _NoiseTex;

#define FOG_STEPPING _RayMarchSettings.x
#define DITHERING _RayMarchSettings.y
#define JITTERING _RayMarchSettings.z
#define MIN_STEPPING _RayMarchSettings.w

float SampleSceneDepth(float2 uv) {
    return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv));
}


inline float GetLinearEyeDepth(float2 uv) {
    float rawDepth = SampleSceneDepth(uv);
	float sceneLinearDepth = LinearEyeDepth(rawDepth);
    #if defined(ORTHO_SUPPORT)
        #if UNITY_REVERSED_Z
              rawDepth = 1.0 - rawDepth;
        #endif
        float orthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
        sceneLinearDepth = lerp(sceneLinearDepth, orthoDepth, unity_OrthoParams.w);
    #endif
    return sceneLinearDepth;
}

#if defined(STEREO_CUBEMAP_RENDER_ON)

sampler2D_float _ODSWorldTexture;

void ClampRayDepth(float3 rayStart, float4 scrPos, inout float t1) {
	float2 uv = scrPos.xy / scrPos.w;
	float3 wpos = tex2D(_ODSWorldTexture, uv);
    float z = distance(rayStart, wpos.xyz);
    t1 = min(t1, z);
}

#else

void ClampRayDepth(float3 rayStart, float4 scrPos, inout float t1) {

    float2 uv =  scrPos.xy / scrPos.w;
    float vz = GetLinearEyeDepth(uv);
    #if defined(ORTHO_SUPPORT)
        if (unity_OrthoParams.w) {
            t1 = min(t1, vz);
            return;
        }
    #endif
    float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    float2 suv = uv;
    #if UNITY_SINGLE_PASS_STEREO
        // If Single-Pass Stereo mode is active, transform the
        // coordinates to get the correct output UV for the current eye.
        float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
        suv = (suv - scaleOffset.zw) / scaleOffset.xy;
    #endif
    float3 vpos = float3((suv * 2 - 1) / p11_22, -1) * vz;
    float4 wpos = mul(unity_CameraToWorld, float4(vpos, 1));

    float z = distance(rayStart, wpos.xyz);
    t1 = min(t1, z);
}

#endif

#endif // VOLUMETRIC_LIGHTS_COMMONS

