using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//////Enum Property//////
[System.Serializable]
public enum DebugPass {
        RayCast = 4,
        Resolve = 1,
		Temporal = 2,
		Combien = 3,
};

[System.Serializable]
public enum RenderResolution {
        Full = 1,
        Half = 2,
};
//////Enum Property//////

[RequireComponent(typeof(Camera))]
public class StochasticScreenSpaceReflection : MonoBehaviour {

//////////////////////////////////////////////////////////////////////////////Property Star///////////////////////////////////////////////////////////////////////////////////////////

	//////BackDepthProperty Star//////
		private Shader BackDepthShader;
		private Camera BackDepthRenderCamera;
		private RenderTexture BackDepthRT;
	//////BackDepthProperty End//////

/* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* */
	
	//////SSR Property Star//////
		private int m_SampleIndex = 0;
		private const int k_SampleCount = 64;
		private Vector2 jitterSample = new Vector2(1, 1);
		private Vector2 CameraSize = new Vector2(Screen.width, Screen.height);
		private Matrix4x4 projectionMatrix;
		private Matrix4x4 viewProjectionMatrix;
		private Matrix4x4 inverseViewProjectionMatrix;
		private Matrix4x4 worldToCameraMatrix;
		private Matrix4x4 cameraToWorldMatrix;
		private Camera RenderCamera;
		private CommandBuffer ScreenSpaceReflectionBuffer;
	//////SSR Property End//////

/* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* */
	
	//////ShaderID Star//////
		private int _Noise = Shader.PropertyToID("_Noise");
		private int _NoiseSize = Shader.PropertyToID("_NoiseSize");
		private int _BRDFBias = Shader.PropertyToID("_BRDFBias");
		private int _NumSteps = Shader.PropertyToID("_NumSteps");
		private int _ScreenFade = Shader.PropertyToID("_ScreenFade");
		private int _Thickness = Shader.PropertyToID("_Thickness");
		private int _ScreenSize = Shader.PropertyToID("_ScreenSize");
		private int _RayCastSize = Shader.PropertyToID("_RayCastSize");
		private int _ResolveSize = Shader.PropertyToID("_ResolveSize");

		private int _SceneColor_RT = Shader.PropertyToID("_SceneColor_RT");
		private int _BackFaceRT = Shader.PropertyToID("_BackFaceRT");
		private int _CombienReflection_RT = Shader.PropertyToID("_CombienReflection_RT");
		private int _Resolve_RT = Shader.PropertyToID("_Resolve_RT");
		private int _TemporalBefore = Shader.PropertyToID("_TemporalBefore");
		private int _TemporalAfter = Shader.PropertyToID("_TemporalAfter");
		private int _RayCastRT = Shader.PropertyToID("_RayCastRT");
		private int _RayPDF_RT = Shader.PropertyToID("_RayPDF_RT");
		private RenderTexture[] _RayCast_RayPDF_RT = new RenderTexture[2];
		private RenderTargetIdentifier[] _RayCast_RayPDF_ID = new RenderTargetIdentifier[2];

		private int _JitterSizeAndOffset = Shader.PropertyToID("_JitterSizeAndOffset");
		private int _ProjectionMatrix = Shader.PropertyToID("_ProjectionMatrix");
		private int _ViewProjectionMatrix = Shader.PropertyToID("_ViewProjectionMatrix");
		private int _InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");
		private int _InverseViewProjectionMatrix = Shader.PropertyToID("_InverseViewProjectionMatrix");
		private int _WorldToCameraMatrix = Shader.PropertyToID("_WorldToCameraMatrix");
		private int _CameraToWorldMatrix = Shader.PropertyToID("_CameraToWorldMatrix");
	//////ShaderID End//////

/* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* *//* */

	//////Control Property Star//////
	[Header("TraceProperty")]
		[SerializeField]
		RenderResolution Resolution = RenderResolution.Half;

		[Range(1, 128)]
    	[SerializeField]
    	int RayMarchingSteps = 58;

		[Range(0, 1)]
    	[SerializeField]
    	float BRDFBias = 0.7f;

		//[Range(0, 1)]
        [SerializeField]
        float Thickness = 0.00025f;

		[Range(0, 1)]
        [SerializeField]
        float FadeSize = 0.05f;

		Material StochasticScreenSpaceReflectionMaterial;

	[Header("TemporalProperty")]

		[Range(0, 6)]
		[SerializeField]
        float TemporalScale = 2;

        [Range(0, 1)]
        [SerializeField]
        float TemporalResponse = 0.72f;

		Texture noise;

	[Header("BackDepthProperty")]
		[SerializeField]
		bool DrawBackDepth = false;

	[Header("DeBugProperty")]

		[SerializeField]
		DebugPass SSRDeBug = DebugPass.Combien;

		[SerializeField]
		bool RunTimeDebugMod = false;

		[SerializeField]
		bool DisableFPSLimit = false;

	//////Control Property End//////

//////////////////////////////////////////////////////////////////////////////Property End///////////////////////////////////////////////////////////////////////////////////////////

	void Awake() {
		RenderCamera = GetComponent<Camera>();
		StochasticScreenSpaceReflectionMaterial = new Material(Shader.Find("Hidden/StochasticScreenSpaceReflection"));
		BackDepthShader = Resources.Load("RenderBackDepth") as Shader;
		noise = Resources.Load("BlueNoise") as Texture2D;

		//////DeBug FPS//////
		SetFPSFrame(DisableFPSLimit, -1);

		//////Install RenderBuffer//////
		InstallRenderCommandBuffer();

		//////Update don't need Tick Refresh Variable//////
		UpdateVariable(false);
	}

	void OnPreCull() {
        jitterSample = GenerateRandomOffset();
    }

	void OnPreRender() {
		//////Update need Tick Refresh Variable//////
		if(RunTimeDebugMod == true){
			UpdateVariable(false);
		}	
		UpdateVariable(true);

		//////Render sceneBackdepth//////
		if(DrawBackDepth == true){
			RenderBackDepth();
		}

		//////RayTrace Buffer//////
		SSRBuffer();
	}

	void OnDisable() {
        RelaseVariable();
    }

	private float GetHaltonValue(int index, int radix) {
        float result = 0f;
        float fraction = 1f / (float)radix;

        while (index > 0) {
            result += (float)(index % radix) * fraction;
            index /= radix;
            fraction /= (float)radix;
        }
        return result;
    }

	private Vector2 GenerateRandomOffset() {
        var offset = new Vector2(GetHaltonValue(m_SampleIndex & 1023, 2), GetHaltonValue(m_SampleIndex & 1023, 3));
        if (m_SampleIndex++ >= k_SampleCount)
        	m_SampleIndex = 0;
        return offset;
    }

	private void UpdateVariable(bool UseTickUpdate) {
		if (UseTickUpdate == false){
			CameraSize = new Vector2(Screen.width/(int)Resolution, Screen.height/(int)Resolution);
			StochasticScreenSpaceReflectionMaterial.SetVector(_ScreenSize, CameraSize);
        	StochasticScreenSpaceReflectionMaterial.SetVector(_RayCastSize, CameraSize);
        	StochasticScreenSpaceReflectionMaterial.SetVector(_ResolveSize, CameraSize);
			StochasticScreenSpaceReflectionMaterial.SetTexture(_Noise, noise);
			StochasticScreenSpaceReflectionMaterial.SetVector(_NoiseSize, new Vector2(noise.width, noise.height));
			StochasticScreenSpaceReflectionMaterial.SetFloat(_BRDFBias, BRDFBias);
			StochasticScreenSpaceReflectionMaterial.SetInt(_NumSteps, RayMarchingSteps);
			StochasticScreenSpaceReflectionMaterial.SetFloat(_ScreenFade, FadeSize);
			StochasticScreenSpaceReflectionMaterial.SetFloat(_Thickness, Thickness);
			StochasticScreenSpaceReflectionMaterial.SetFloat("_TScale", TemporalScale);
            StochasticScreenSpaceReflectionMaterial.SetFloat("_TResponse", TemporalResponse);
			
		} else {
			StochasticScreenSpaceReflectionMaterial.SetVector(_JitterSizeAndOffset, new Vector4 ((float)CameraSize.x / (float)noise.width, (float)CameraSize.y / (float)noise.height, jitterSample.x, jitterSample.y));

			worldToCameraMatrix = RenderCamera.worldToCameraMatrix;
        	cameraToWorldMatrix = worldToCameraMatrix.inverse;
        	projectionMatrix = GL.GetGPUProjectionMatrix(RenderCamera.projectionMatrix, false);
        	viewProjectionMatrix = projectionMatrix * worldToCameraMatrix;

        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_ProjectionMatrix, projectionMatrix);
        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_ViewProjectionMatrix, viewProjectionMatrix);
        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_InverseProjectionMatrix, projectionMatrix.inverse);
        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_InverseViewProjectionMatrix, viewProjectionMatrix.inverse);
        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_WorldToCameraMatrix, worldToCameraMatrix);
        	StochasticScreenSpaceReflectionMaterial.SetMatrix(_CameraToWorldMatrix, cameraToWorldMatrix);

		}
	}

	private void RenderBackDepth() {
	if (BackDepthRenderCamera == null) {
		var BackCamera = new GameObject();
		BackCamera.transform.SetParent(RenderCamera.transform);
		BackCamera.hideFlags = HideFlags.HideAndDontSave;
		BackDepthRenderCamera = BackCamera.AddComponent<Camera>();
		BackDepthRenderCamera.CopyFrom(RenderCamera);
		BackDepthRenderCamera.enabled = false;
		BackDepthRenderCamera.clearFlags = CameraClearFlags.SolidColor;
		BackDepthRenderCamera.backgroundColor = Color.black;
		BackDepthRenderCamera.renderingPath = RenderingPath.Forward;
		BackDepthRenderCamera.SetReplacementShader(BackDepthShader, "RenderType");
			
		BackDepthRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 8, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		BackDepthRT.filterMode = FilterMode.Point;
		BackDepthRenderCamera.targetTexture = BackDepthRT;
		Shader.SetGlobalTexture(_BackFaceRT, BackDepthRT);
	}	
		BackDepthRenderCamera.Render();     
    }	

	private void InstallRenderCommandBuffer() {
		//////Install MultiRT on RayCast Buffer//////
		_RayCast_RayPDF_RT[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
		_RayCast_RayPDF_RT[1] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.RHalf);
		
		//////Install RayCastingBuffer//////
		ScreenSpaceReflectionBuffer = new CommandBuffer();
		ScreenSpaceReflectionBuffer.name = "StochasticScreenSpaceReflection";
		RenderCamera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, ScreenSpaceReflectionBuffer);
	}

	private void SSRBuffer() {
		ScreenSpaceReflectionBuffer.Clear();

		//////Set SceneColor//////
		ScreenSpaceReflectionBuffer.GetTemporaryRT(_SceneColor_RT, Screen.width, Screen.height, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
		ScreenSpaceReflectionBuffer.Blit(BuiltinRenderTextureType.CameraTarget, _SceneColor_RT);

		//////Render rayCasting(HitUV.x, HitUV.y, RayDepth, RayCastingMask)//////
		_RayCast_RayPDF_ID[0] = _RayCast_RayPDF_RT[0].colorBuffer;
		_RayCast_RayPDF_ID[1] = _RayCast_RayPDF_RT[1].colorBuffer;

		ScreenSpaceReflectionBuffer.SetGlobalTexture(_RayCastRT, _RayCast_RayPDF_RT[0]);
		ScreenSpaceReflectionBuffer.SetGlobalTexture(_RayPDF_RT, _RayCast_RayPDF_RT[1]);
		ScreenSpaceReflectionBuffer.BlitMRT(_RayCast_RayPDF_ID, BuiltinRenderTextureType.CameraTarget, StochasticScreenSpaceReflectionMaterial, 0);

		//////Resolve rayCasting at 4 time//////
		ScreenSpaceReflectionBuffer.GetTemporaryRT(_Resolve_RT, Screen.width, Screen.height, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
		ScreenSpaceReflectionBuffer.Blit(_Resolve_RT, _Resolve_RT, StochasticScreenSpaceReflectionMaterial, 1);

		//////Use temporalFiltter//////
		ScreenSpaceReflectionBuffer.GetTemporaryRT(_TemporalBefore, Screen.width, Screen.height, 8, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
		ScreenSpaceReflectionBuffer.SetGlobalTexture(_TemporalBefore, _TemporalBefore);

		ScreenSpaceReflectionBuffer.GetTemporaryRT(_TemporalAfter, Screen.width, Screen.height, 8, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
		ScreenSpaceReflectionBuffer.Blit(_Resolve_RT, _TemporalAfter, StochasticScreenSpaceReflectionMaterial, 2);

		ScreenSpaceReflectionBuffer.Blit(_TemporalAfter, _TemporalBefore);

		//////Combien Reflection//////
		ScreenSpaceReflectionBuffer.GetTemporaryRT(_CombienReflection_RT, Screen.width, Screen.height, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
		ScreenSpaceReflectionBuffer.Blit(_CombienReflection_RT, BuiltinRenderTextureType.CameraTarget, StochasticScreenSpaceReflectionMaterial, (int)SSRDeBug);
	}

	private void RelaseVariable() {
		if(BackDepthRT != null){
			RenderTexture.ReleaseTemporary(BackDepthRT);
		}
		if(_RayCast_RayPDF_RT[0] != null){
			RenderTexture.ReleaseTemporary(_RayCast_RayPDF_RT[0]);
		}
		if(_RayCast_RayPDF_RT[1] != null){
			RenderTexture.ReleaseTemporary(_RayCast_RayPDF_RT[1]);
		}

		ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(_Resolve_RT);
		ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(_TemporalBefore);
		ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(_TemporalAfter);
		ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(_SceneColor_RT);
		ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(_CombienReflection_RT);
		RenderCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, ScreenSpaceReflectionBuffer);
		ScreenSpaceReflectionBuffer.Release();
		ScreenSpaceReflectionBuffer.Dispose();


	}

	private void SetFPSFrame(bool UseHighFPS, int TargetFPS) {
		if(UseHighFPS == true){
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = TargetFPS;
		}
		else{
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = 60;
		}
	}
}
