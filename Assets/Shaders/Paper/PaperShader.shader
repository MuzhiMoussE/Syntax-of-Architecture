// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Book/PaperShaderWithShadow"
{
    Properties
    {
    //材质属性 BlinnPhong模型中需要的三个常量参数
    _Diffuse("Diffuse",Color)=(1,1,1,1)    //漫反射中所需的diffuse值(漫反射系数)
    _Specular("Specular",Color)=(1,1,1,1)  //高光反射中所需的specular值(高光反射系数)
    _Gloss("Gloss",Range(8.0,256))=20  //高光反射中所需的材质光泽度
    _Color1 ("颜色1", Color) = (1,1,1,1)
    _Color2 ("颜色2", Color) = (0.0, 1.0, 0.0, 1.0)
	[PowerSlider(1)] _Pos1("第二个颜色位置", Range(0.0, 1.0)) = 0.2
    _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
 
        Pass{
            Tags{"LightMode"="ForwardBase"}
 
            CGPROGRAM
            //使用该指令能确保使用光照衰减等光照变量能被正确赋值
            #pragma multi_compile_fwdbase  
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            //添加该内置文件以便于使用计算阴影用的宏
            #include "AutoLight.cginc"
 
            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
 
            struct a2v{
                float4 vertex:POSITION;
                float4 normal:NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f{
                float4 pos:SV_POSITION;
                float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
                //添加一个内置宏来声明一个用于对阴影纹理采样的坐标,括号中的参数需要是下一个可用的插值寄存器的索引值,比如上一个texcoord1，改插值寄存器索引值就为2
                SHADOW_COORDS(2)
                float2 texcoord : TEXCOORD3;
            	float2 uv : TEXCOORD4;
            };
            sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color1;
            fixed4 _Color2;
            float _Pos1;
            v2f vert(a2v v){
                v2f o;
                //得到裁剪空间下的顶点坐标
                o.pos=UnityObjectToClipPos(v.vertex);
                //得到世界坐标系下的模型法线和顶点坐标
                o.worldNormal=mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
                //添加该宏用于在顶点着色器中计算上一步声明的阴影纹理坐标
                o.texcoord = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
                TRANSFER_SHADOW(o);
                return o;
            }
 
            fixed4 frag(v2f i):SV_Target{
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
                //计算环境光
                fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
 
                //利用兰伯特公式计算漫反射
                fixed3 worldNormal=normalize(i.worldNormal);
                fixed3 worldLightDir=normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse=_LightColor0.rgb*_Diffuse.rgb*saturate(dot(worldNormal,worldLightDir));
 
                //计算高光反射
                fixed3 viewDir=normalize(_WorldSpaceCameraPos-i.worldPos.xyz);
                fixed3 halfDir=normalize(viewDir+worldLightDir);
                fixed3 specular=_LightColor0.rgb*_Specular*pow(saturate(dot(worldNormal,halfDir)),_Gloss);
 
                //计算阴影值
                //fixed shadow=SHADOW_ATTENUATION(i);
                
                //计算光照衰减
                //fixed atten=1.0;
 
                //使用内置宏来计算光照衰减和阴影
                UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
 
                //然后来计算最终产生的阴影
                return fixed4(ambient+(diffuse+specular)*atten,1.0)*c;
 
            }
                
            ENDCG
        }
 
        Pass {
			// Pass for other pixel lights
			Tags { "LightMode"="ForwardAdd" }
			
			Blend One One
		
			CGPROGRAM
			
			// Apparently need to add this declaration
			#pragma multi_compile_fwdadd
			// Use the line below to add shadows for point and spot lights
//			#pragma multi_compile_fwdadd_fullshadows
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};
			
			v2f vert(a2v v) {
			 	v2f o;
			 	o.pos = UnityObjectToClipPos(v.vertex);
			 	
			 	o.worldNormal = UnityObjectToWorldNormal(v.normal);
			 	
			 	o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			 	
			 	// Pass shadow coordinates to pixel shader
			 	TRANSFER_SHADOW(o);
			 	
			 	return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				
			 	fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

			 	fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
			 	fixed3 halfDir = normalize(worldLightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

				// UNITY_LIGHT_ATTENUATION not only compute attenuation, but also shadow infos
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
			 	
				return fixed4((diffuse + specular) * atten, 1.0);
			}
			
			ENDCG
		}
    }
    FallBack "Diffuse"
}