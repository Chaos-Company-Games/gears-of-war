#ifndef CEL_MAIN_LIGHT_INCLUDED
#define CEL_MAIN_LIGHT_INCLUDED

void GetMainLightData_float(float3 worldPos, float4 shadowCoord, out float3 Direction, out float3 Color, out float ShadowAtten)
{
    Light light = GetMainLight(shadowCoord);
    Direction = light.direction;
    Color = light.color;
    ShadowAtten = light.shadowAttenuation;
}

void GetMainLightData_half(half3 worldPos, half4 shadowCoord, out half3 Direction, out half3 Color, out half ShadowAtten)
{
    Light light = GetMainLight(shadowCoord);
    Direction = light.direction;
    Color = light.color;
    ShadowAtten = light.shadowAttenuation;
}

#endif