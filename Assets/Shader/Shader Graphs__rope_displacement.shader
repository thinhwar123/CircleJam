Shader "Shader Graphs/_rope_displacement" {
	Properties {
		[NoScaleOffset] _Texture2D ("Texture2D", 2D) = "white" {}
		_Tiling ("Tiling", Vector) = (4,15,0,0)
		_Rope_Detail_1_Color ("Rope Detail 1 Color", Vector) = (0,0.1688426,0.972549,0)
		_Rope_Detail_2_Color ("Rope Detail 2 Color", Vector) = (0.1372549,0.9529412,1,1)
		_Second_Detail_Color_1 ("Second Detail Color 1", Vector) = (0,0.6886792,0.08138929,1)
		_Second_Detail_Color_2 ("Second Detail Color 2", Vector) = (0.3075848,0.990566,0.2196066,1)
		_Third_Detail_Color_1 ("Third Detail Color 1", Vector) = (0.6901961,0,0.08154025,1)
		_Third_Detail_Color_2 ("Third Detail Color 2", Vector) = (1,0.5512537,0.504717,1)
		_EdgeSize ("EdgeSize", Range(0.5, 3)) = 1.5
		_NormalStrength ("NormalStrength", Range(0, 0.5)) = 0.2
		_InnerThickness ("InnerThickness", Float) = 0.1
		_ThirdColorLenght ("ThirdColorLenght", Float) = 0.15
		_DirtyLength ("DirtyLength", Float) = 1
		_DirtColor1 ("DirtColor1", Vector) = (0.6980392,0.3137255,0.1333333,0)
		_DirtColor2 ("DirtColor2", Vector) = (0.2358491,0.1078522,0.0789872,0)
		_DirtMaskSlider1 ("DirtMaskSlider1", Float) = 0.18
		_DirtMaskSlider2 ("DirtMaskSlider2", Float) = 0
		_DirtFlowSpeed ("DirtFlowSpeed", Range(0, 1)) = 0.3
		_DissolveSize ("DissolveSize", Float) = 4
		_DirtTiling ("DirtTiling", Vector) = (1.5,5,0,0)
		[NoScaleOffset] _SampleTexture2D_d05c646e6db747569d7dd7d4fc3d7da8_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] [Normal] _SampleTexture2D_f6b764d982fc47a292aaeee73cb851f3_Texture_1_Texture2D ("Texture2D", 2D) = "bump" {}
		[NoScaleOffset] _SampleTexture2D_0404900be81844c29e4f4b30147a19e6_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] [Normal] _SampleTexture2D_2847af27a92e46d1b74d8d4e9fc2904a_Texture_1_Texture2D ("Texture2D", 2D) = "bump" {}
		[NoScaleOffset] _SampleTexture2D_f7021fbe8bb94c58888bc6f29bb8edcf_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
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