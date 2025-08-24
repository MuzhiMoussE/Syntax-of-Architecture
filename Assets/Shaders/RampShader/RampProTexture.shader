// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Book/LineColorTexture"
{
    Properties
    {
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { }
		_Color1 ("颜色1", Color) = (1,1,1,1)
    	_Color2 ("颜色2", Color) = (0.0, 1.0, 0.0, 1.0)
		[PowerSlider(1)] _Pos1("第二个颜色位置", Range(0.0, 1.0)) = 0.2
    	_MainTex ("Texture", 2D) = "white" {}
    	//材质属性 BlinnPhong模型中需要的三个常量参数
		_Diffuse("Diffuse",Color)=(1,1,1,1)    //漫反射中所需的diffuse值(漫反射系数)
		_Specular("Specular",Color)=(1,1,1,1)  //高光反射中所需的specular值(高光反射系数)
		_Gloss("Gloss",Range(8.0,256))=20  //高光反射中所需的材质光泽度

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
        	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
            #include "AutoLight.cginc"
			
            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float3 worldPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				fixed3 diff : COLOR0;
				fixed3 ambient : COLOR1;
				fixed3 intensity : COLOR2;
				float2 disRim : TEXCOORD4;
				float3 cubNor : TEXCOORD5;
				float2 texcoord : TEXCOORD6;
            	float2 uv : TEXCOORD7;
            	SHADOW_COORDS(2)
            };
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			samplerCUBE _ToonShade;

			fixed4 _Color1;
            fixed4 _Color2;
            float _Pos1;
			float4 _ScaleTex;
			float4 _Divid;
			
			float Sigmoid(float x, float center, float sharp) {
				return 1 / (1 + pow(100000, (-3 * sharp * (x - center))));
			}
            v2f vert (appdata v)
            {
                v2f o;
                
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 dir = normalize(worldPos - _WorldSpaceCameraPos);
				float3 map = cross(dir, worldNormal);
				map = mul((float3x3)UNITY_MATRIX_V, map);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				half3 shadow = ShadeSH9(half4(worldNormal,1));
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = worldPos;
				o.normal = abs(worldNormal);
				o.diff = nl * _LightColor0.rgb;
				o.ambient = shadow;
				o.intensity = Sigmoid(o.diff.x + o.ambient.x, _Divid.x, _Divid.w);
				o.cubNor = worldNormal;
				o.texcoord = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				TRANSFER_SHADOW(o);
                return o;
            }
			
            fixed4 frag(v2f i) : SV_Target
			{
			    fixed shadow = SHADOW_ATTENUATION(i);
			    fixed4 cube = texCUBE(_ToonShade, i.cubNor);
			    fixed4 _col; // 像素颜色
			    float lp = 0.0;

			    if (i.uv.y >= _Pos1)
			    {
			        lp = (1 - i.uv.y) / (1 - _Pos1);
			        _col = lerp(_Color1, _Color2, lp);
			    }
			    else
			    {
			        _col = _Color2;  // 否则就使用第二个颜色
			    }

			    fixed4 col = tex2D(_MainTex, i.texcoord);
			    fixed4 c = col * _col;
			    c *= shadow;  // 将阴影衰减乘以颜色

			    return c;
			}
			ENDCG
        	
		}
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
