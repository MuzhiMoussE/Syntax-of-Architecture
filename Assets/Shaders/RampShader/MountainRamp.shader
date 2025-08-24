// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Book/MountainColorTexture"
{
    Properties
    {
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { }
		_Color1 ("颜色1", Color) = (1,1,1,1)
    	_Color2 ("颜色2", Color) = (0.0, 1.0, 0.0, 1.0)
    	_Color3("线条颜色",Color) = (1.0,1.0,0.5,1.0)
		[PowerSlider(1)] _Pos1("第二个颜色位置", Range(0.0, 1.0)) = 0.2
    	_MainTex ("Texture", 2D) = "white" {}
		
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
				float2 texcoord : TEXCOORD6;
            	float2 uv : TEXCOORD7;
            	SHADOW_COORDS(8)

            };
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			samplerCUBE _ToonShade;
			
			fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            float _Pos1;
			fixed4 _RimColor;
			float4 _ScaleTex;
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
				half nol = nl + shadow;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = worldPos;
				o.normal = abs(worldNormal);

				
				o.diff = nl * _LightColor0.rgb;
				o.ambient = shadow;
				
				o.texcoord = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				TRANSFER_SHADOW(o);
                return o;
            }
			
            fixed4 frag (v2f i) : SV_Target
            {
            	 fixed shadow=SHADOW_ATTENUATION(i);
            	//像素颜色
                fixed4 _col;
                //插值
                float lp = 0.0;
				if (i.uv.y >= _Pos1)
				{
					// 插值 = 当前像素在pos1范围内的比重 = 在当前像素点的y值 / _Pos1界限值 
					lp = (1 - i.uv.y) / (1 - _Pos1);
					//像素输出的颜色 = _Color1 - _Color2之间，处于lp位置的颜色
					//lerp 三个参数为起始值，终止值，比重
					_col = lerp(_Color1, _Color2, lp);
				}
                // 否则就使用第二个颜色
				else
				{
					_col = _Color2;
				}
            	fixed4 texColor = tex2D(_MainTex, i.texcoord);
				float alphaThreshold = 0.5; // 设置透明度阈值
				float4 col;

				if (texColor.r != 1.0 && texColor.g!=1.0 && texColor.b!=1.0)
				{
				    col = texColor;
				}
				else
				{
				    col = texColor * _col;
				}
				if (texColor.a < alphaThreshold)
				{
				    col = texColor * _col;
				    
				    // apply fog
				    UNITY_APPLY_FOG(i.fogCoord, col);
				    
				    col = col*shadow;
				}
				else
				{
				    clip(1); // 剔除当前像素
				    col = _Color3 * shadow; // 返回原始贴图颜色
				}
            	return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
