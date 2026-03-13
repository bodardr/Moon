float4 SnapToPixelCoords(float4 positionOS, float4x4 objectToWorld)
{
	float3 objectOriginWS = objectToWorld._m03_m13_m23;
	float4 objectOriginHClip = TransformWorldToHClip(objectOriginWS);
	float2 objectOriginNDC = objectOriginHClip.xy * 0.5f + 0.5f;
	float2 snappedObjectOriginNDC = floor(objectOriginNDC * _ScreenParams.xy) / _ScreenParams.xy;

	float2 snappingDelta = (objectOriginNDC - snappedObjectOriginNDC) * 2.0f;
	return TransformObjectToHClip(positionOS) - float4(snappingDelta.x, snappingDelta.y, 0, 0);
}

void SnapToPixelCoords_float(float4 positionOS, float4x4 objectToWorld, out float4 positionOSSnapped)
{
	positionOSSnapped = SnapToPixelCoords(positionOS, objectToWorld);
}
