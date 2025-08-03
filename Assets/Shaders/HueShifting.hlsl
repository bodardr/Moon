#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

void HueShift_float(float3 medianColor, float3 lightColor, float lightIntensity, float3 hueShiftingParams, out float3 outColor)
{
	float3 medianHSV = RgbToHsv(medianColor);
	float3 lightHSV = RgbToHsv(lightColor);
	float3 hsvDelta = frac(lightHSV - medianHSV);

	float3 resultHSV = float3(0, 0, 0);
	resultHSV.x = frac(medianHSV.x + hsvDelta.x * lightIntensity * hueShiftingParams.x);
	resultHSV.y = medianHSV.y + (1 - lightIntensity) * hueShiftingParams.y;
	resultHSV.z = medianHSV.z * (lightIntensity * 2 - 1) * hueShiftingParams.z;

	outColor = HsvToRgb(resultHSV);
}