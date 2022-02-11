#ifndef VOLUMETRIC_LIGHTS_CUSTOM_SHADOW
#define VOLUMETRIC_LIGHTS_CUSTOM_SHADOW

sampler2D_float _ShadowTexture;
float4x4 _ShadowMatrix;

float4 shadowTextureStart;
float4 shadowTextureEnd;
half3 _ShadowIntensity;

sampler2D _Cookie2D;


inline float4 GetShadowCoords(float3 wpos) {
    return mul(_ShadowMatrix, float4(wpos, 1.0));
}

void ComputeShadowTextureCoords(float3 rayStart, float3 rayDir, float t0, float t1) {
    shadowTextureStart = GetShadowCoords(rayStart + rayDir * t0);
    shadowTextureEnd = GetShadowCoords(rayStart + rayDir * t1);
}

float TestShadowMap(float4 shadowCoords) {
    float shadowDepth = tex2Dlod(_ShadowTexture, float4(shadowCoords.xy, 0, 0)).r; 
    #if UNITY_REVERSED_Z
        shadowCoords.z = shadowCoords.w - shadowCoords.z;
        shadowDepth = shadowCoords.w - shadowDepth;
    #endif
    #if VL_POINT
        shadowCoords.z = clamp(shadowCoords.z, -shadowCoords.w, shadowCoords.w);
    #endif    
    float shadowTest = shadowCoords.z<0 || shadowDepth > shadowCoords.z;
    return shadowTest;
}


inline half3 UnitySpotCookie(float4 lightCoord) {
    half4 cookie = tex2Dlod(_Cookie2D, lightCoord);
    return cookie.rgb;
}

inline half3 GetShadowTerm(float4 shadowCoords) {

    #if VL_SPOT_COOKIE
        half3 s = UnitySpotCookie(shadowCoords);
    #else
        half3 s = 1.0.xxx;
    #endif

    #if VL_SHADOWS
        half sm = TestShadowMap(shadowCoords);
        sm = sm * _ShadowIntensity.x + _ShadowIntensity.y; 
        s *= sm;
    #endif

    return s;
}

half3 GetShadowAtten(float x) {
    float4 shadowCoords = lerp(shadowTextureStart, shadowTextureEnd, x);
    shadowCoords.xyz /= shadowCoords.w;
    return GetShadowTerm(shadowCoords);
}


half3 GetShadowAttenWS(float3 wpos) {
    float4 shadowCoords = mul(_ShadowMatrix, float4(wpos, 1.0));
    shadowCoords.xyz /= shadowCoords.w;
    return GetShadowTerm(shadowCoords);
}


half3 GetShadowAttenParticlesWS(float3 wpos) {
    float4 shadowCoords = mul(_ShadowMatrix, float4(wpos, 1.0));
    shadowCoords.xyz /= shadowCoords.w;

    #if VL_SPOT_COOKIE
        half3 shadowTest = UnitySpotCookie(shadowCoords);
    #else
        half3 shadowTest = 1.0.xxx;
    #endif

    #if VL_SHADOWS
        shadowTest *= TestShadowMap(shadowCoords);
        // ignore particles outside of shadow map
        float inMap = all(shadowCoords.xy > 0.0.xx && shadowCoords.xy < 1.0.xx);
        shadowTest *= (half)inMap;
    #endif

    return shadowTest;
}


#endif // VOLUMETRIC_LIGHTS_CUSTOM_SHADOW

