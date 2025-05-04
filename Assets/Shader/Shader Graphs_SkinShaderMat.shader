Shader "Shader Graphs/SkinShaderMat" {
	Properties {
		_Color_1 ("Color 1", Vector) = (0,0,0,0)
		_Color_2 ("Color 2", Vector) = (1,1,1,0)
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		_Rotate ("Rotate", Float) = 90
		_RimLight_Smooth ("RimLight Smooth", Float) = 1
		_RimLight_Strength ("RimLight Strength", Float) = 1
		_TextureTile ("TextureTile", Float) = 1
		[NoScaleOffset] Texture2D_63A047D1 ("Texture", 2D) = "white" {}
		[NoScaleOffset] _MatcapTexture ("MatcapTexture", 2D) = "white" {}
		[HDR] _Matcap_Color ("Matcap Color", Vector) = (1,1,1,0)
		_Active ("Active", Range(0, 1)) = 0.5
		_TextureTiling ("TextureTiling", Vector) = (1,1,0,0)
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}