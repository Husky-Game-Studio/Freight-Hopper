#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
	Direction = float3(0.5, 0.5, 0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
#else
	float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	DistanceAtten = mainLight.distanceAttenuation;

#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
	ShadowAtten = 1.0h;
#else
	ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
	float shadowStrength = GetMainLightShadowStrength();
	ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
		sampler_MainLightShadowmapTexture),
		shadowSamplingData, shadowStrength, false);
#endif
#endif
}
#endif