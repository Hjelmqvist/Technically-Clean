Shader "VolumetricLights/DustParticles" {

Properties {
     _MainTex ("Particle Texture", 2D) = "white" {}
	[HideInInspector] _ParticleDistanceAtten ("Distance Atten", Float) = 10
	[HideInInspector] _ParticleTintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		[HideInInspector] _BoundsCenter("Bounds Center", Vector) = (0,0,0)
		[HideInInspector] _BoundsExtents("Bounds Size", Vector) = (0,0,0)
		[HideInInspector] _ConeTipData("Cone Tip Data", Vector) = (0,0,0,0.1)
		[HideInInspector] _ExtraGeoData("Extra Geo Data", Vector) = (1.0, 0, 0)
        [HideInInspector] _Border("Border", Float) = 0.1
        [HideInInspector] _DistanceFallOff("Length Falloff", Float) = 0
        [HideInInspector] _FallOff("FallOff Physical", Vector) = (1.0, 2.0, 1.0)
        [HideInInspector] _ConeAxis("Cone Axis", Vector) = (0,0,0,0.5)
        [HideInInspector] _AreaExtents("Area Extents", Vector) = (0,0,0,1)
        [HideInInspector] _ShadowIntensity("Shadow Intensity", Vector) = (0,1,0,0)
		[HideInInspector] _Cookie2D("Cookie (2D)", 2D) = "black" {}
}

SubShader {

    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha One
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

        Pass {

            CGPROGRAM
    		#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local VL_SPOT VL_SPOT_COOKIE VL_POINT VL_AREA_RECT VL_AREA_DISC 
            #pragma multi_compile_local _ VL_SHADOWS
            #pragma multi_compile_local _ VL_CUSTOM_BOUNDS
    		#pragma multi_compile_local _ VL_PHYSICAL_ATTEN
            #pragma multi_compile_instancing
            #include "DustParticlesForwardPass.hlsl"
            ENDCG
        }
    }
}
