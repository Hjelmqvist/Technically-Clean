Shader "Custom/WallShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "white" {}
        _HeightTex ("Height Map", 2D) = "white" {}
        _Height ("Height", Range(0,1)) = 0.5
        _OcclusionTex ("Occlusion", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", 2D) = "black" {}

        _DitherPattern ("Dither Pattern", 2D) = "white" {}
        _MinTrans ("Minimum Transparency", Range(0.001, 1.0)) = 0.05
        _MaxTrans ("Maximum Transparency", Range(0.001, 1.0)) = 0.5
        _PlayerPos ("Player Position", Vector) = (100,100,0)
        _PlayerSize ("Players Size", Vector) = (.6,1,1)
        _TransOffset ("Offset of Transparency", Vector) = (0,.2,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalTex;
        sampler2D _HeightTex;
        sampler2D _OcclusionTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float2 uv_HeightMap;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        half _Height;
        fixed4 _Color;

        sampler2D _DitherPattern;
        float4 _DitherPattern_TexelSize;
        float _MinTrans;
        float _MaxTrans;
        float _ScreenRatio;

        float3 _PlayerPos;
        float3 _PlayerSize;
        float2 _TransOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float EllipseDistance(float2 vec1, float2 vec2, float2 ellipseShape)
        {
            float2 subtraction = abs(vec1 - vec2);

            float2 thing = subtraction / ellipseShape;

            return length(thing);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
            float2 playerSize = _PlayerSize / IN.screenPos.w;
            float transValue = EllipseDistance(screenPos - _TransOffset, _PlayerPos, playerSize);
            transValue = saturate(transValue);
            transValue = abs(1 - transValue);
            transValue = (transValue - _MinTrans) / (_MaxTrans - _MinTrans);

            float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
            float4 ditherCol = tex2D(_DitherPattern, ditherCoordinate);
            float ditherValue = (ditherCol.r + ditherCol.g + ditherCol.b + ditherCol.a) / 4;

            clip(ditherValue - transValue - .001);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            float2 texOffset = ParallaxOffset(tex2D(_HeightTex, IN.uv_MainTex).r, _Height, IN.viewDir);
            o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex + texOffset));
            o.Occlusion = tex2D(_OcclusionTex, IN.uv_MainTex);
        }
        ENDCG
    }
    FallBack "Diffuse"
}