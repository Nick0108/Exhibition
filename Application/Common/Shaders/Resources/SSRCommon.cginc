#include "UnityStandardBRDF.cginc"
#define PI 3.141592

float sqr(float x) {
	return x*x;
}
	
float fract(float x) {
	return x - floor( x );
}

float ComputeDepth(float4 clippos) {
	#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
		return (clippos.z / clippos.w) * 0.5 + 0.5;
	#else
		return clippos.z / clippos.w;
	#endif
}

float3 GetViewNormal (float3 normal, float4x4 _WToCMatrix) {
	float3 viewNormal =  mul((float3x3)_WToCMatrix, normal.rgb);
	return normalize(viewNormal);
}

float LinearDepthReverseBack(float Depth){
	float z =  ((1 /  Depth) - _ZBufferParams.y) / _ZBufferParams.x;
	#if defined(UNITY_REVERSED_Z)
		z = 1 - z;
	#endif
	return z;
}

float GetDepth (sampler2D tex, float2 uv) {
	float z = tex2Dlod(tex, float4(uv,0,0));
	#if defined(UNITY_REVERSED_Z)
		z = 1 - z;
	#endif
	return z;
}

float Get01Depth (sampler2D tex, float2 uv) {
	float z = Linear01Depth(tex2Dlod(tex, float4(uv,0,0)).r);
	#if defined(UNITY_REVERSED_Z)
		z = 1 - z;
	#endif
	return z;
}

float GetEyeDepth (sampler2D tex, float2 uv) {
	float z = LinearEyeDepth(tex2Dlod(tex, float4(uv,0,0)).r);
	#if defined(UNITY_REVERSED_Z)
		z = 1 - z;
	#endif
	return z;
}

float3 GetSSRScreenPos (float2 uv, float depth) {
	return float3(uv.xy * 2 - 1, depth);
}

float3 GetWorlPos (float3 screenPos, float4x4 _InverseViewProjectionMatrix)
{
	float4 worldPos = mul(_InverseViewProjectionMatrix, float4(screenPos, 1));
	return worldPos.xyz / worldPos.w;
}

float3 GetViewRayFromUV(float2 uv, float4x4	_ProjectionMatrix) {
	float4 _CamScreenDir = float4(1 / _ProjectionMatrix[0][0], 1 / _ProjectionMatrix[1][1], 1, 1);
	float3 ray = float3(uv.x * 2 - 1, uv.y * 2 - 1, 1);
	ray *= _CamScreenDir.xyz;
	ray = ray * (_ProjectionParams.z / ray.z);
	return ray;
}

float3 GetViewPos (float3 screenPos, float4x4 _InverseProjectionMatrix) {
	float4 viewPos = mul(_InverseProjectionMatrix, float4(screenPos, 1));
	return viewPos.xyz / viewPos.w;
}
	
float3 GetViewDir (float3 worldPos, float3 ViewPos) {
	return normalize(worldPos - ViewPos);
}

static const float2 offset[4] = {
	float2(0, 0),
	float2(2, -2),
	float2(-2, -2),
	float2(0, 2)
};

float RayAttenBorder (float2 pos, float value) {
	float borderDist = min(1 - max(pos.x, pos.y), min(pos.x, pos.y));
	return saturate(borderDist > value ? 1 : borderDist / value);
}

#define SCREEN_EDGE_MASK 0.9
float alphaCalc(float3 rayDirection, float2 hitPixel, float marchPercent, float hitZ) {
	float res = 1;
	res *= saturate(-5 * (rayDirection.z - 0.2));
	float2 screenPCurrent = 2 * (hitPixel - 0.5);
	res *= 1 - max(
	(clamp(abs(screenPCurrent.x), SCREEN_EDGE_MASK, 1.0) - SCREEN_EDGE_MASK) / (1 - SCREEN_EDGE_MASK),
	(clamp(abs(screenPCurrent.y), SCREEN_EDGE_MASK, 1.0) - SCREEN_EDGE_MASK) / (1 - SCREEN_EDGE_MASK)
	);
	res *= 1 - marchPercent;
	res *= 1 - (-(hitZ - 0.2) * _ProjectionParams.w);
	return res;
}

float3 BinarySearch(inout float3 rayDir, inout float3 samplePos, inout float sampleDepth, float4x4 projection, sampler2D depth) {

    float4 projectedCoord;
    float forntDepth;
	
	UNITY_LOOP
    for(int i = 0; i < 6; i++) {
        projectedCoord = mul(projection, float4(samplePos, 1));
        projectedCoord.xy /= projectedCoord.w;
        projectedCoord.xy = projectedCoord.xy * 0.5 + 0.5;

        forntDepth = 1 - tex2Dlod(depth, float4(projectedCoord.xy, 0, 0)).x;
        sampleDepth = samplePos.z - forntDepth;

        rayDir *= 0.5;
        if(sampleDepth > 0)
            samplePos += rayDir;
        else
            samplePos -= rayDir;   
    }
        projectedCoord = mul(projection, float4(samplePos, 1));
        projectedCoord.xy /= projectedCoord.w;
        projectedCoord.xy = projectedCoord.xy * 0.5 + 0.5;
 
    return float3(projectedCoord.xy, sampleDepth);
}

half2 CalculateMotion(float rayDepth, float2 inUV, float4x4 _InverseViewProjectionMatrix, float4x4 _PrevViewProjectionMatrix, float4x4 _ViewProjectionMatrix) {
	float3 screenPos = GetSSRScreenPos(inUV, rayDepth);
	float4 worldPos = float4(GetWorlPos(screenPos, _InverseViewProjectionMatrix),1);

	float4 prevClipPos = mul(_ViewProjectionMatrix, worldPos);
	float4 curClipPos = mul(_ViewProjectionMatrix, worldPos);

	float2 prevHPos = prevClipPos.xy / prevClipPos.w;
	float2 curHPos = curClipPos.xy / curClipPos.w;

	float2 vPosPrev = (prevHPos.xy + 1.0f) / 2.0f;
	float2 vPosCur = (curHPos.xy + 1.0f) / 2.0f;
	return vPosCur - vPosPrev;
}

bool Intersect_Thickness(sampler2D forntDepthTex, sampler2D backDepthTex, float2 HitUV, float RayStarDepth, float RayEndDepth) {
    float ForntDepth = GetDepth(forntDepthTex, HitUV);
    float BackDepth = LinearDepthReverseBack(tex2Dlod(backDepthTex, float4(HitUV, 0, 0)).r);
    if (RayStarDepth > RayEndDepth) {
        float t = RayStarDepth;
        RayStarDepth = RayEndDepth;
        RayEndDepth = t;
    }
    return RayStarDepth < BackDepth && RayEndDepth > ForntDepth;
	//return RayEndDepth > ForntDepth && RayEndDepth < BackDepth;
}

float4 RayMarch_Thickness(sampler2D _BackFaceRT, sampler2D tex, float4x4 _ProjectionMatrix, float3 reflectionDir, int NumSteps, float3 viewPos, float3 screenPos, float2 screenUV, float stepSize) {
	float4 dirProject = float4 (
		abs(unity_CameraProjection._m00 * 0.5), 
		abs(unity_CameraProjection._m11 * 0.5), 
		((_ProjectionParams.z * _ProjectionParams.y) / (_ProjectionParams.y - _ProjectionParams.z)) * 0.5,
		0
	);

	float eyeDepth = LinearEyeDepth(tex2D(tex, screenUV.xy));
	float3 ray = viewPos / viewPos.z;
	float3 rayDir = normalize(float3(reflectionDir.xy - ray * reflectionDir.z, reflectionDir.z / eyeDepth) * dirProject);
	rayDir.xy *= 0.5;

	float3 rayStart = float3(screenPos.xy * 0.5 + 0.5,  screenPos.z);
	float3 samplePos = rayStart;
	float sampleDepth = rayStart.z;

	float mask;

	UNITY_LOOP
	for (int i = 0;  i < NumSteps; i++) {
		if (Intersect_Thickness(tex, _BackFaceRT, samplePos.xy, rayStart.z, sampleDepth)) {
			mask = 1.0;
			break;
		}
		samplePos += rayDir * stepSize;
		sampleDepth = samplePos.z;
	}
	return float4(samplePos, saturate(mask*12));
}

bool Intersect_Linear(sampler2D forntDepthTex, float2 HitUV, float RayStarDepth, float RayEndDepth, float Thickness) {
    float ForntDepth = GetDepth(forntDepthTex, HitUV);
    float BackDepth = ForntDepth + Thickness;
    if (RayStarDepth > RayEndDepth) {
        float t = RayStarDepth;
        RayStarDepth = RayEndDepth;
        RayEndDepth = t;
    }
    return RayStarDepth < BackDepth && RayEndDepth > ForntDepth;
}

float4 RayMarch_Linear(sampler2D _BackFaceRT, sampler2D tex, float4x4 _ProjectionMatrix, float3 reflectionDir, int NumSteps, float3 viewPos, float3 screenPos, float2 screenUV, float stepSize, float thickness) {
	float4 dirProject = float4 (
		abs(unity_CameraProjection._m00 * 0.5), 
		abs(unity_CameraProjection._m11 * 0.5), 
		((_ProjectionParams.z * _ProjectionParams.y) / (_ProjectionParams.y - _ProjectionParams.z)) * 0.5,
		0
	);

	float eyeDepth = LinearEyeDepth(tex2D(tex, screenUV.xy));
	float3 ray = viewPos / viewPos.z;
	float3 rayDir = normalize(float3(reflectionDir.xy - ray * reflectionDir.z, reflectionDir.z / eyeDepth) * dirProject);
	rayDir.xy *= 0.5;

	float3 rayStart = float3(screenPos.xy * 0.5 + 0.5,  screenPos.z);
	float3 samplePos = rayStart;
	float sampleDepth = rayStart.z;
	float mask;

	UNITY_LOOP
	for (int i = 0;  i < NumSteps; i++) {
		if (Intersect_Linear(tex, samplePos.xy, rayStart.z, sampleDepth, thickness)) {
			mask = 1.0;
			break;
		}
		samplePos += rayDir * stepSize;
		sampleDepth = samplePos.z;
	}
	return float4(samplePos, mask);
}

float4 RayMarch_Maxwell(sampler2D _BackFaceRT, sampler2D tex, float4x4 _ProjectionMatrix, float3 reflectionDir, int NumSteps, float3 viewPos, float3 screenPos, float2 screenUV, float stepSize, float thickness) {
	float4 dirProject = float4 (
		abs(unity_CameraProjection._m00 * 0.5), 
		abs(unity_CameraProjection._m11 * 0.5), 
		((_ProjectionParams.z * _ProjectionParams.y) / (_ProjectionParams.y - _ProjectionParams.z)) * 0.5,
		0
	);

	float eyeDepth = LinearEyeDepth(tex2D(tex, screenUV.xy));
	float3 ray = viewPos / viewPos.z;
	float3 rayDir = normalize(float3(reflectionDir.xy - ray * reflectionDir.z, reflectionDir.z / eyeDepth) * dirProject);
	rayDir.xy *= 0.5;

	float3 rayStart = float3(screenPos.xy * 0.5 + 0.5,  screenPos.z);
	float3 samplePos = rayStart;
	float sampleDepth = rayStart.z;

	float mask;
	float forntDepth, backDepth;

	float minimum = 1;
	float3 index = 0;
	UNITY_LOOP
	for(int i = 0; i < NumSteps; i++) {
		samplePos += rayDir * stepSize;
		sampleDepth = samplePos.z;
		forntDepth = GetDepth(tex, samplePos.xy);
		float diff = abs(forntDepth - sampleDepth);
		if(diff < minimum) {
			minimum = diff;
			index = samplePos;
		}
	}
	mask = minimum < thickness;
	return float4(index, mask);
}

/*
#ifndef _YRC_SCREEN_SPACE_RAYTRACE_
#define _YRC_SCREEN_SPACE_RAYTRACE_
#define RAY_LENGTH 128	 

bool RayIntersect(float raya, float rayb, float2 sspt, sampler2D _backDepth, sampler2D _CameraDepth) {
	if (raya > rayb) {
		float t = raya;
		raya = rayb;
		rayb = t;
	}
	float backZ = tex2Dlod(_backDepth, float4(sspt / 2 + 0.5, 0, 0)).r;
	#if 1
		float screenPCameraDepth = -LinearEyeDepth(tex2Dlod(_CameraDepth, float4(sspt / 2 + 0.5, 0, 0)).r);
		return raya < screenPCameraDepth && rayb > screenPCameraDepth - backZ;
	#else
		return raya < backZ && rayb > screenPCameraDepth;
	#endif
}

bool RayMarch_TP(float3 start, float3 direction, float4 texelSize, out float2 hitPixel, out float marchPercent,out float hitZ, sampler2D _backDepth, sampler2D _CameraDepth, float Steps, float2 jitter) {
	float rayLength = ((start.z + direction.z * RAY_LENGTH) > -_ProjectionParams.y) ?
		(-_ProjectionParams.y - start.z) / direction.z : RAY_LENGTH;

	float3 end = start + direction * rayLength;

	float4 H0 = mul(unity_CameraProjection, float4(start, 1));
	float4 H1 = mul(unity_CameraProjection, float4(end, 1));

	float2 screenP0 = H0.xy / H0.w;
	float2 screenP1 = H1.xy / H1.w;	

	float k0 = 1 / H0.w;
	float k1 = 1 / H1.w;

	float Q0 = start.z * k0;
	float Q1 = end.z * k1;

	if (abs(dot(screenP1 - screenP0, screenP1 - screenP0)) < 0.00001) {
		screenP1 += texelSize.xy;
	}
	float2 deltaPixels = (screenP1 - screenP0) * texelSize.zw;
	float step;	
	step = min(1 / abs(deltaPixels.y), 1 / abs(deltaPixels.x));
	step *= Steps;	

	float sampleScaler = 1 - min(1, -start.z / 100);
	step *= 1 + sampleScaler;	

	float interpolationCounter = step;
	float4 pqk = float4(screenP0, Q0, k0);
	float4 dpqk = float4(screenP1 - screenP0, Q1 - Q0, k1 - k0) * step;
	pqk += float4(jitter, 0, 0) * dpqk;
	float prevZMaxEstimate = start.z;

	bool intersected = false;
	UNITY_LOOP	
	for (int i = 1; i <= 64 && interpolationCounter <= 1 && !intersected; i++, interpolationCounter += step) {
		pqk += dpqk;
		float rayZMin = prevZMaxEstimate;
		float rayZMax = ( pqk.z) / ( pqk.w);

		if (RayIntersect(rayZMin, rayZMax, pqk.xy - dpqk.xy / 2, _backDepth, _CameraDepth)) {
			hitPixel = (pqk.xy - dpqk.xy / 2) / 2 + 0.5;
			marchPercent = (float)i / 64;
			intersected = true;
		} else {
			prevZMaxEstimate = rayZMax;
		}
	}
	#if 1	
	if (intersected) {
		pqk -= dpqk;	
		UNITY_LOOP
		for (float gapSize = Steps; gapSize > 1.0; gapSize /= 2) {
			dpqk /= 2;
			float rayZMin = prevZMaxEstimate;
			float rayZMax = (pqk.z) / ( pqk.w);

			if (RayIntersect(rayZMin, rayZMax, pqk.xy - dpqk.xy / 2, _backDepth, _CameraDepth)) {	

			} else {			
				pqk += dpqk;
				prevZMaxEstimate = rayZMax;
			}
		}
		hitPixel = (pqk.xy - dpqk.xy / 2) / 2 + 0.5;
	}
	#endif
	hitZ = pqk.z / pqk.w;
	return intersected;
}
#endif
*/