Shader "Hidden/StochasticScreenSpaceReflection" {
	Properties {
		
	}

	CGINCLUDE
		#include "SSRPassCommon.cginc"
	ENDCG

	SubShader {
		ZTest Always 
		Cull Off 
		ZWrite Off

		Pass {
			Name"RayCastingPass"
			CGPROGRAM
				#pragma vertex vertCustom
				//#pragma fragment RayCasting_Thickness
				#pragma fragment RayCasting_Linear
			ENDCG
		} Pass {
			Name"ResolvePass"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment Resolve
			ENDCG
		} Pass {
			Name"TemporalPass"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment Temporal
			ENDCG
		} Pass {
			Name"CombienPass"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment Combien
			ENDCG
		} Pass {
			Name"DeBugRayCast"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment DeBugRayCast
			ENDCG
		}
	}
}
