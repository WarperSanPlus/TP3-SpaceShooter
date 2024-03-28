Shader "Custom/ScrollingTexture" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextureScroll ("Texture X Scroll", Range(0, 10)) = 5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _TextureScroll;

		struct Input {
			float2 uv_MainTex;
			float3 uv_NormalTex;
			float3 vertColor;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 scrolledUV = IN.uv_MainTex;
			float scrollSpeed = _TextureScroll * _Time;
			scrolledUV += float2(0, 0.01 * scrollSpeed);
		
			half4 c = tex2D (_MainTex, scrolledUV);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
