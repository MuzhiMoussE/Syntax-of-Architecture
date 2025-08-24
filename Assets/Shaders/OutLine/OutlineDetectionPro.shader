Shader "Book/OutlineDetectionPro" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {} // 主纹理
		_EdgeOnly ("Edge Only", Float) = 1.0 // 是否仅显示边缘
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1) // 边缘颜色
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1) // 背景颜色
		_SampleScale("Sample Scale", Float) = 1.0 // 采样缩放系数(值越大, 描边越宽)
		_DepthScale("Depth Scale", Float) = 1.0 // 深度缩放系数(值越大, 越易识别为边缘)
		_NormalScale("Normal Scale", Float) = 1.0 // 法线缩放系数(值越大, 越易识别为边缘)
	}
 
	SubShader {
		Pass {
			// 深度测试始终通过, 关闭深度写入
			ZTest Always ZWrite Off
 
			CGPROGRAM
			
			#include "UnityCG.cginc"
			
			#pragma vertex vert  
			#pragma fragment frag
			
			sampler2D _MainTex; // 主纹理
			sampler2D _CameraDepthNormalsTexture; // 深度&法线纹理
			uniform half4 _MainTex_TexelSize;  // _MainTex的像素尺寸大小, float4(1/width, 1/height, width, height)
			fixed _EdgeOnly; // 是否仅显示边缘
			fixed4 _EdgeColor; // 边缘颜色
			fixed4 _BackgroundColor; // 背景颜色
			float _SampleScale; // 采样缩放系数(值越大, 描边越宽)
			float _DepthScale; // 深度缩放系数(值越大, 越易识别为边缘)
			float _NormalScale; // 法线缩放系数(值越大, 越易识别为边缘)
 
			struct v2f {
				float4 pos : SV_POSITION; // 裁剪空间中顶点坐标
				half2 uv[5] : TEXCOORD0; // 顶点及其周围4个角的uv坐标
			};
 
			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv[0] = v.texcoord;
				o.uv[1] = v.texcoord + _MainTex_TexelSize.xy * half2(1, 1) * _SampleScale;
				o.uv[2] = v.texcoord + _MainTex_TexelSize.xy * half2(-1, -1) * _SampleScale;
				o.uv[3] = v.texcoord + _MainTex_TexelSize.xy * half2(-1, 1) * _SampleScale;
				o.uv[4] = v.texcoord + _MainTex_TexelSize.xy * half2(1, -1) * _SampleScale;
				return o;
			}
			
			bool isEdge(half4 sample1, half4 sample2) { // 是否是边缘(1: 是, 0: 否)
				// 计算深度差
				float depth1 = DecodeFloatRG(sample1.zw); // 观察空间中的线性且归一化的深度
				float depth2 = DecodeFloatRG(sample2.zw); // 观察空间中的线性且归一化的深度
				float depthDelta = abs(depth1 - depth2) * depth1 * _DepthScale; // 深度差异(距离相机越远, 像素点越少, 对深度差越敏感, 所以乘了depth1)
				bool isDepthDiff = depthDelta > 0.01; // 深度是否不同
				// 计算法线差
				float3 normal1 = DecodeViewNormalStereo(sample1); // 观察空间中的法线向量
				float3 normal2 = DecodeViewNormalStereo(sample2); // 观察空间中的法线向量
				float normalDelta = (1 - abs(dot(normal1, normal2))) * _NormalScale; // 法线差异
				bool isNormalDiff = normalDelta > 0.14; // cos(30°)=0.86, 法线夹角小于30°
				return isDepthDiff || isNormalDiff;
			}
 
			fixed4 frag(v2f i) : SV_Target {
				half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
				half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
				half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
				half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4]);
				bool isDiff = isEdge(sample1, sample2); // 是否是边缘
				isDiff = isDiff && isEdge(sample3, sample4);
				if (isDiff) {
					return _EdgeColor;
				}
				fixed4 tex = tex2D(_MainTex, i.uv[0]);
				return lerp(tex, _BackgroundColor, _EdgeOnly);
 			}
			
			ENDCG
		} 
	}
 
	FallBack Off
}