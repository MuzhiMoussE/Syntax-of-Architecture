Shader "Book/WindGrass"
{
	Properties
	{
		[Header(Wind)]
		[Space]
		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", Range(0, 2)) = 1
		[Header(Density)]
		[Space]
		_Density("Density", Range(1, 30)) = 5
		[Header(Color of grass)]
		[Space]
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		[Header(Shape of blade)]
		[Space]
		_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
		_BladeWidth("Blade Width", Range(0, 0.2)) = 0.1
		_BladeWidthRandom("Blade Width Random", Range(0, 0.1)) = 0.02
		_BladeHeight("Blade Height", Range(0, 1)) = 0.5
		_BladeHeightRandom("Blade Height Random", Range(0, 1)) = 0.3
		_BladeForward("Blade Forward Amount", Range(0, 1)) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
	}
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AutoLight.cginc"

		#define ADD_TANSPCVERT(v, matrix) \
				o.pos = UnityObjectToClipPos(pos + mul(matrix, v)); \
				o.uv = float2(v.x - 0.5, v.y); \
				triStream.Append(o); 

		#define BLADE_SEGMENTS 3

		struct a2v
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		struct v2g
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		struct g2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		float _BendRotationRandom;
		float _BladeHeight;
		float _BladeHeightRandom;
		float _BladeWidth;
		float _BladeWidthRandom;
		float _Density;
		sampler2D _WindDistortionMap;
		float4 _WindDistortionMap_ST;
		float2 _WindFrequency;
		float _WindStrength;
		float _BladeForward;
		float _BladeCurve;

		v2g vert(a2v v)
		{
			v2g o;
			o.vertex = v.vertex;
			o.normal = v.normal;
			o.tangent = v.tangent;
			return o;
		}

		float rand(float3 seed)
		{
			float f = sin(dot(seed, float3(127.1, 337.1, 256.2)));
			f = -1 + 2 * frac(f * 43785.5453123);
			return f;
		}

		float2 randto2D(float2 seed)
		{
			float2 f = sin(float2(dot(seed, float2(127.1, 337.1)), dot(seed, float2(269.5, 183.3))));
			f = frac(f * 43785.5453123);
			return f;
		}

		float3x3 AngleAxis3x3(float angle, float3 axis)
		{
			float s, c;
			sincos(angle, s, c);
			float x = axis.x;
			float y = axis.y;
			float z = axis.z;
			return float3x3(
				x * x + (y * y + z * z) * c, x * y * (1 - c) - z * s, x * z * (1 - c) - y * s,
				x * y * (1 - c) + z * s, y * y + (x * x + z * z) * c, y * z * (1 - c) - x * s,
				x * z * (1 - c) - y * s, y * z * (1 - c) + x * s, z * z + (x * x + y * y) * c
			);
		}

		void addVert(float3 pos, float3x3 tangentToObject, inout TriangleStream<g2f> triStream)
		{
			float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos.xyz) * UNITY_TWO_PI, float3(0, 0, 1));
			float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zyx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
			float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
			float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * max(_WindStrength, 0.0001);
			float3 wind = normalize(float3(windSample.x, windSample.y, 0));
			float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);
			float3x3 transformationMatrixFacing = mul(tangentToObject, facingRotationMatrix);
			tangentToObject = mul(mul(mul(tangentToObject, windRotation), facingRotationMatrix), bendRotationMatrix);

			float height = max(rand(pos.xzy) * _BladeHeightRandom + _BladeHeight, 0.1);
			float width = max(rand(pos.yzx) * _BladeWidthRandom + _BladeWidth, 0.01);
			float forward = rand(pos.yyz) * _BladeForward;
			g2f o;
			for (int i = 0; i < BLADE_SEGMENTS; i++)
			{
				float t = i / (float)BLADE_SEGMENTS;
				float segmentHeight = height * t;
				float segmentWidth = width * (1 - t);
				float segmentForward = pow(t, _BladeCurve) * forward;
				float3x3 transformationMatrix = i == 0 ? transformationMatrixFacing : tangentToObject;
				ADD_TANSPCVERT(float3(segmentWidth / 2, segmentForward, segmentHeight), transformationMatrix);
				ADD_TANSPCVERT(float3(-segmentWidth / 2, segmentForward, segmentHeight), transformationMatrix);
			}
			ADD_TANSPCVERT(float3(0, forward, height), tangentToObject);
			triStream.RestartStrip();
		}

		[maxvertexcount((BLADE_SEGMENTS * 2 + 1) * 24)]
		void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
		{
			float3 normal = IN[0].normal;
			float4 tangent = IN[0].tangent;
			float3 biNormal = cross(normal, tangent) * tangent.w;
			float3x3 tangentToObject = float3x3(
				tangent.x, biNormal.x, normal.x,
				tangent.y, biNormal.y, normal.y,
				tangent.z, biNormal.z, normal.z
			);
			float4 center = (IN[0].vertex + IN[1].vertex + IN[2].vertex) / 3;//center of quad
			float3 pos = center.xyz;
			for (int i = 0; i < _Density; i++)
			{
				float2 offset = randto2D(pos.xz);
				pos = IN[0].vertex;
				pos += (IN[1].vertex - pos) * offset.x;
				pos += (IN[2].vertex - pos) * offset.y;
				addVert(pos.xyz, tangentToObject, triStream);
			}
		}
		ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			Cull Off
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			
			fixed4 _TopColor;
			fixed4 _BottomColor;
			
			fixed4 frag(g2f i) : SV_Target
			{
				fixed4 color = lerp(_BottomColor, _TopColor, i.uv.y);
				return color;
			}
			ENDCG
		}      
		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.0
			#pragma multi_compile_shadowcaster

			float4 frag(g2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
    }
    FallBack "Diffuse"
}

