#include "UnityCG.cginc"
//#include "UnityPBSLighting.cginc"
//#include "UnityStandardBRDF.cginc"
//#include "UnityStandardUtils.cginc"
#include "SSRCommon.cginc"
#include "NoiseCommon.cginc"
#include "BRDFCommon.cginc"

uniform sampler2D _CameraDepthTexture, _BackFaceRT, _SceneColor_RT, _Noise, _RayCastRT, _RayPDF_RT, _CameraMotionVectorsTexture, _Resolve_RT, 
				  _TemporalBefore, _TemporalAfter, _CameraGBufferTexture0, _CameraGBufferTexture1, _CameraGBufferTexture2, _CameraReflectionsTexture;

uniform int _NumSteps;
										 
uniform float _BRDFBias, _ScreenFade, _TScale, _TResponse, _Thickness;

uniform float4 _ScreenSize, _NoiseSize,
				_ResolveSize, _RayCastSize, _JitterSizeAndOffset;
																					
uniform float4x4 _ProjectionMatrix, _ViewProjectionMatrix, _InverseProjectionMatrix,
				 _InverseViewProjectionMatrix, _WorldToCameraMatrix, _CameraToWorldMatrix;

struct VertexInput {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct PixelInput {
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
};
	
PixelInput vertCustom(VertexInput v) {
	PixelInput o;
	o.vertex = v.vertex;
	o.uv = v.uv;
	return o;
}

PixelInput vert(VertexInput v) {
	PixelInput o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	return o;
}

void RayCasting_Thickness (PixelInput i, out float4 rayCasting : SV_Target0, out half rayPDFs : SV_Target1){	
	float2 uv = i.uv;
	float4 normal = tex2D(_CameraGBufferTexture2, uv);
	float roughness = 1 - tex2D(_CameraGBufferTexture1, uv).a;
	float4 worldNormal = normal * 2 - 1;
	
	float depth = GetDepth(_CameraDepthTexture, uv);
	float3 screenPos = GetSSRScreenPos(uv, depth);
	float3 worldPos = GetWorlPos(screenPos, _InverseViewProjectionMatrix);
	float3 viewPos = GetViewPos(screenPos, _InverseProjectionMatrix);
	float3 viewDir = GetViewDir(worldPos, _WorldSpaceCameraPos);

	float2 jitter = tex2Dlod(_Noise, float4((uv + _JitterSizeAndOffset.zw) * _RayCastSize.xy / _NoiseSize.xy, 0, -255)).xy; 
	float2 Xi = jitter;
	Xi.y = lerp(Xi.y, 0, _BRDFBias);

	float4 rayPDF = TangentToWorld(worldNormal, ImportanceSampleGGX(Xi, roughness));
	float3 reflectionDir = reflect(viewDir, rayPDF.xyz);
	reflectionDir = normalize(mul((float3x3)_WorldToCameraMatrix, reflectionDir));
	jitter += 0.5;
	float stepSize = (1 / (float)_NumSteps);
	stepSize = stepSize * (jitter.x + jitter.y) + stepSize;

	float4 rayCast = RayMarch_Thickness(_BackFaceRT, _CameraDepthTexture, _ProjectionMatrix, reflectionDir, _NumSteps, viewPos, screenPos, uv, stepSize);
	
	rayCasting = rayCast;
	rayPDFs = rayPDF.a;
}

void RayCasting_Linear (PixelInput i, out float4 rayCasting : SV_Target0, out half rayPDFs : SV_Target1){	
	float2 uv = i.uv;
	float4 normal = tex2D(_CameraGBufferTexture2, uv);
	float roughness = 1 - tex2D(_CameraGBufferTexture1, uv).a;

	float Thickness = _Thickness;
	float4 worldNormal = normal * 2 - 1;
	
	float depth = GetDepth(_CameraDepthTexture, uv);
	float3 screenPos = GetSSRScreenPos(uv, depth);
	float3 worldPos = GetWorlPos(screenPos, _InverseViewProjectionMatrix);
	float3 viewPos = GetViewPos(screenPos, _InverseProjectionMatrix);
	float3 viewDir = GetViewDir(worldPos, _WorldSpaceCameraPos);

	float2 jitter = tex2Dlod(_Noise, float4((uv + _JitterSizeAndOffset.zw) * _RayCastSize.xy / _NoiseSize.xy, 0, -255)).xy; 
	float2 Xi = jitter;
	Xi.y = lerp(Xi.y, 0.0, _BRDFBias);
	float4 rayPDF = TangentToWorld(worldNormal, ImportanceSampleGGX(Xi, roughness));
	float3 reflectionDir = reflect(viewDir, rayPDF.xyz);
	reflectionDir = normalize(mul((float3x3)_WorldToCameraMatrix, reflectionDir));
	jitter += 0.5f;
	float stepSize = (1.0 / (float)_NumSteps);
	stepSize = stepSize * (jitter.x + jitter.y) + stepSize;

	float4 rayCast = RayMarch_Linear(_BackFaceRT, _CameraDepthTexture, _ProjectionMatrix, reflectionDir, _NumSteps, viewPos, screenPos, uv, stepSize, Thickness);
	
	rayCasting = rayCast;
	rayPDFs = rayPDF.a;
}

/*
float4 _MainTex_TexelSize;
void RayCasting_TP (PixelInput i, out float4 rayCasting : SV_Target0, out half rayPDFs : SV_Target1){	
	float2 uv = i.uv;
	float4 normal = tex2D(_CameraGBufferTexture2, uv);
	float roughness = 1 - tex2D(_CameraGBufferTexture1, uv).a;
	
	float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);
	float4 screenPos = float4(i.uv * 2 - 1, 1, 1);
	screenPos = mul(unity_CameraInvProjection, screenPos);
	float3 Ray = screenPos / screenPos.w;
	float3 RayOrigin = depth * Ray;
	float4 worldNormal = normal * 2 - 1;

	float2 jitter = tex2Dlod(_Noise, float4((uv + _JitterSizeAndOffset.zw) * _RayCastSize.xy / _NoiseSize.xy, 0, -255)).xy; 
	float2 Xi = jitter;
	Xi.y = lerp(Xi.y, 0, _BRDFBias);
	float4 rayPDF = TangentToWorld(worldNormal, ImportanceSampleGGX(Xi, roughness));
	float3 viewNormal = normalize(mul((float3x3)_WorldToCameraMatrix, rayPDF.xyz));
	float3 dir = reflect(RayOrigin, viewNormal.xyz);
	jitter += 0.5;
	float stepSize = (1 / (float)_NumSteps);
	stepSize = stepSize * (jitter.x + jitter.y) + stepSize;

	float2 hitPixel;
	float3 debugCol;
	float alpha, marchPercent, hitZ;

	float rayBump = max(-0.018*RayOrigin.z, 0.001);
	if (RayMarch_TP(RayOrigin + viewNormal * rayBump, dir, _MainTex_TexelSize, hitPixel, marchPercent, hitZ, _BackFaceRT, _CameraDepthTexture, 12, jitter)) {
		alpha = alphaCalc(dir, hitPixel, marchPercent, hitZ );
		alpha = RayAttenBorder (hitPixel, _ScreenFade) * alpha;
	}

	rayCasting = float4(hitPixel, hitZ/64, saturate(alpha*12));
	rayPDFs = rayPDF.a;
}
*/

float4 Resolve(PixelInput i) : SV_Target {
	float2 uv = i.uv;
	int2 pos = uv * _ScreenSize.xy;
	float4 Normal = tex2D(_CameraGBufferTexture2, uv);
	float Roughness = (1 - tex2D(_CameraGBufferTexture1, uv).a);
			
	float4 worldNormal = Normal * 2 - 1;
	float3 viewNormal = GetViewNormal (worldNormal, _WorldToCameraMatrix);

	float Depth = GetDepth(_CameraDepthTexture, uv);
	float3 screenPos = GetSSRScreenPos(uv, Depth);
	float3 worldPos = GetWorlPos(screenPos, _InverseViewProjectionMatrix);
	float3 viewPos = GetViewPos(screenPos, _InverseProjectionMatrix);
	float3 viewDir = GetViewDir(worldPos, viewPos);

	float2 Noise = tex2D(_Noise, (uv + _JitterSizeAndOffset.zw) * _ScreenSize.xy / _NoiseSize.xy) * 2 - 1;
	float2x2 OffsetRotationMatrix = float2x2(Noise.x, Noise.y, -Noise.y, Noise.x);

	float NdotV = saturate(dot(worldNormal, -viewDir));
	float coneTangent = lerp(0, Roughness * (1 - _BRDFBias), NdotV * sqrt(Roughness));

	float weightSum, hitZ, hitMask, weight, intersectionCircleRadius, mip, hitPDF;
	float2 offsetUV, neighborUv, hitUv;
	float3 hitViewPos;
	float4 hitPacked, sampleColor, reflecttionColor;

	UNITY_LOOP
    for(int i = 0; i < 4; i++) {
		offsetUV = offset[i] * (1 / _ResolveSize.xy);
		offsetUV =  mul(OffsetRotationMatrix, offsetUV);
		neighborUv = uv + offsetUV;

        hitPacked = tex2Dlod(_RayCastRT, float4(neighborUv, 0, 0));
		hitPDF = tex2Dlod(_RayPDF_RT, float4(neighborUv, 0, 0));
        hitUv = hitPacked.xy;
        hitZ = hitPacked.z;
		hitMask = hitPacked.a;
		hitViewPos = GetViewPos(GetSSRScreenPos(hitUv, hitZ), _InverseProjectionMatrix);

		weight =  BRDF_UE4(normalize(-viewPos) /*V*/, normalize(hitViewPos - viewPos) /*L*/, viewNormal /*N*/, max(0.014, Roughness)) / max(1e-5, hitPDF);

		intersectionCircleRadius = coneTangent * length(hitUv - uv);
		mip = clamp(log2(intersectionCircleRadius * max(_ResolveSize.x, _ResolveSize.y)), 0, 4);

		sampleColor.rgb = tex2Dlod(_SceneColor_RT, float4(hitUv, 0, mip)).rgb;
		sampleColor.a = RayAttenBorder (hitUv, _ScreenFade) * hitMask;
		sampleColor.rgb /= 1 + Luminance(sampleColor.rgb);
		reflecttionColor += sampleColor * weight;
    	weightSum += weight;
    }
	reflecttionColor /= weightSum;
	reflecttionColor.rgb /= 1 - Luminance(reflecttionColor.rgb);
	return max(1e-5, reflecttionColor);
}

float4 Temporal (PixelInput i) : SV_Target {	
	float2 uv = i.uv;
	float3 hitPacked = tex2Dlod(_RayCastRT, float4(uv, 0, 0));
	//float2 velocity = CalculateMotion(hitPacked.z, uv, _InverseViewProjectionMatrix, _ViewProjectionMatrix, _ViewProjectionMatrix);
	float2 velocity = tex2D(_CameraMotionVectorsTexture, uv).xy;

	#if defined(UNITY_REVERSED_Z)
	hitPacked.z = 1 - hitPacked.z;
	#endif
	
	float4 current = tex2D(_Resolve_RT, uv);
	float4 previous = tex2D(_TemporalBefore, uv);

	float2 du = float2(1 / _ScreenSize.x, 0);
	float2 dv = float2(0, 1 / _ScreenSize.y);

	float4 currentTopLeft = tex2D(_Resolve_RT, uv.xy - dv - du);
	float4 currentTopCenter = tex2D(_Resolve_RT, uv.xy - dv);
	float4 currentTopRight = tex2D(_Resolve_RT, uv.xy - dv + du);
	float4 currentMiddleLeft = tex2D(_Resolve_RT, uv.xy - du);
	float4 currentMiddleCenter = tex2D(_Resolve_RT, uv.xy);
	float4 currentMiddleRight = tex2D(_Resolve_RT, uv.xy + du);
	float4 currentBottomLeft = tex2D(_Resolve_RT, uv.xy + dv - du);
	float4 currentBottomCenter = tex2D(_Resolve_RT, uv.xy + dv);
	float4 currentBottomRight = tex2D(_Resolve_RT, uv.xy + dv + du);

	float4 currentMin = min(currentTopLeft, min(currentTopCenter, min(currentTopRight, min(currentMiddleLeft, min(currentMiddleCenter, min(currentMiddleRight, min(currentBottomLeft, min(currentBottomCenter, currentBottomRight))))))));
	float4 currentMax = max(currentTopLeft, max(currentTopCenter, max(currentTopRight, max(currentMiddleLeft, max(currentMiddleCenter, max(currentMiddleRight, max(currentBottomLeft, max(currentBottomCenter, currentBottomRight))))))));

	float4 center = (currentMin + currentMax) * 0.5;
	currentMin = (currentMin - center) * _TScale + center;
	currentMax = (currentMax - center) * _TScale + center;

	previous = clamp(previous, currentMin, currentMax);
    return lerp(current, previous, saturate(clamp(0, 0.96, _TResponse) *  (1 - length(velocity) * 8)));
}

float4 Combien (PixelInput i) : SV_Target {	
	float2 uv = i.uv;
	float4 albedoColor = tex2D(_CameraGBufferTexture0, uv);
	float4 specularColor = tex2D(_CameraGBufferTexture1, uv);
	float Roughness = 1 - specularColor.a;
	float4 Normal = tex2D(_CameraGBufferTexture2, uv);
	float4 worldNormal = Normal * 2 - 1;
	float depth = GetDepth(_CameraDepthTexture, uv);

	float4 SSR = tex2D(_TemporalAfter, uv) * albedoColor.a;
	float SSRMask = sqrt(SSR.a);
	float4 Cubemap =  tex2D(_CameraReflectionsTexture, uv);
	
	float3 screenPos = GetSSRScreenPos(uv, depth);
	float3 worldPos = GetWorlPos(screenPos, _InverseViewProjectionMatrix);
	float3 viewDir = GetViewDir(worldPos, _WorldSpaceCameraPos);
	float NoV = saturate(dot(worldNormal, -viewDir));
	float4 Fresnel = float4(FresnelSchlick(NoV, specularColor), 1);

	float4 reflectionColor = lerp(SSR, Cubemap, saturate(Roughness-0.25)) * Fresnel;
	//float4 reflectionColor = SSR * Fresnel;
	reflectionColor = lerp(Cubemap, reflectionColor, SSRMask);

	float4 sceneColor = tex2D(_SceneColor_RT,  uv);
	sceneColor.rgb = max(1e-5, sceneColor.rgb - Cubemap.rgb);

	return sceneColor + reflectionColor;	
}

float4 DeBugRayCast (PixelInput i) : SV_Target {	
	float2 uv = i.uv;
	return float4(tex2D(_RayCastRT, uv).xyz, 1);
}