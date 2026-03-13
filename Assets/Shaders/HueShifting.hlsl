#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

void HueShift_float(float3 brightColor, float lightIntensity, float3 hueShiftingParams, out float3 outColor)
{
	float3 baseHSV = RgbToHsv(brightColor);
	float  shiftIntensity = 1 - lightIntensity;
	
	const float coldHue = 0.85f;
	
	
	float3 resultHSV = baseHSV;
	resultHSV.x = baseHSV.x + shiftIntensity * hueShiftingParams.x;
	resultHSV.y = baseHSV.y + shiftIntensity * hueShiftingParams.y;
	resultHSV.z = baseHSV.z - shiftIntensity * hueShiftingParams.z;

	outColor = HsvToRgb(resultHSV);
}