float D_GGX(float Roughness, float NdotH) {
	float m = Roughness * Roughness;
	float m2 = m * m;
	
	float D = m2 / (PI * sqr(sqr(NdotH) * (m2 - 1) + 1));
	
	return D;
}

	
float G_GGX(float Roughness, float NdotL, float NdotV) {
	float m = Roughness * Roughness;
	float m2 = m * m;

	float G_L = 1 / (NdotL + sqrt(m2 + (1 - m2) * NdotL * NdotL));
	float G_V = 1 / (NdotV + sqrt(m2 + (1 - m2) * NdotV * NdotV));
	float G = G_L * G_V;
	
	return G;
}

float BRDF_UE4(float3 V, float3 L, float3 N, float Roughness) {
	float3 H = normalize(L + V);

	float NdotH = saturate(dot(N,H));
	float NdotL = saturate(dot(N,L));
	float NdotV = saturate(dot(N,V));

	float D = D_GGX(Roughness, NdotH);
	float G = G_GGX(Roughness, NdotL, NdotV);

	return D * G;
}

float BRDF_Unity_Weight(float3 V, float3 L, float3 N, float Roughness) {
	float3 H = normalize(L + V);

	float NdotH = saturate(dot(N,H));
	float NdotL = saturate(dot(N,L));
	float NdotV = saturate(dot(N,V));

	half G = SmithJointGGXVisibilityTerm (NdotL, NdotV, Roughness);
	half D = GGXTerm (NdotH, Roughness);

	return (D * G) * (UNITY_PI / 4);
}

float4 TangentToWorld(float3 N, float4 H) {
	float3 UpVector = abs(N.z) < 0.999 ? float3(0, 0, 1) : float3(1, 0, 0);
	float3 T = normalize( cross( UpVector, N ) );
	float3 B = cross( N, T );
				 
	return float4((T * H.x) + (B * H.y) + (N * H.z), H.w);
}

float4 ImportanceSampleGGX(float2 Xi, float Roughness) {
	float m = Roughness * Roughness;
	float m2 = m * m;
		
	float Phi = 2 * PI * Xi.x;
				 
	float CosTheta = sqrt((1 - Xi.y) / (1 + (m2 - 1) * Xi.y));
	float SinTheta = sqrt(max(1e-5, 1 - CosTheta * CosTheta));
				 
	float3 H;
	H.x = SinTheta * cos(Phi);
	H.y = SinTheta * sin(Phi);
	H.z = CosTheta;
		
	float d = (CosTheta * m2 - CosTheta) * CosTheta + 1;
	float D = m2 / (PI * d * d);
	float pdf = D * CosTheta;

	return float4(H, pdf); 
}

float3 FresnelSchlick(float NoV, float3 F0) {
    return F0 + (1 - F0) * pow(1.0 - NoV, 5);
}



