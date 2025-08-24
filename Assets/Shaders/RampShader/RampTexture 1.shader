Shader "Custom/lineColor" 
{
	//该shader共3个颜色，可调节三个颜色，与中间颜色的纵向域
	//unity可见的属性
    Properties
    {
         _MainTex ("贴图", 2D) = "white" {}
         _Color1 ("颜色1", Color) = (1.0, 0.0, 0.0, 1.0)
         _Color2 ("颜色2", Color) = (0.0, 1.0, 0.0, 1.0)
		[PowerSlider(1)] _Pos1("第二个颜色位置", Range(0.0, 1.0)) = 0.2
    }
	
    SubShader
    {
        Pass
        { 
        	//透明队列
            Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
            //常规透明混合
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            //定义顶点、片元渲染器，引入工具宏
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			//程序传入顶点渲染器的参数
            struct a2v
            {
                float4 pos: POSITION;
                float2 uv : TEXCOORD0;
            };
			//顶点渲染器传入片元渲染器
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos: SV_POSITION;
            };
			//定义参数：贴图、三个颜色、位置
			sampler2D _MainTex;
            fixed4 _Color1;
            fixed4 _Color2;
			float _Pos1;
			//顶点渲染器  
            v2f vert (a2v v)
            {
                v2f o;
                //顶点坐标 从模型空间转为裁剪空间
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;
                return o;
            }
            //片元渲染器
            fixed4 frag (v2f i) : SV_Target
            {
            	//像素颜色
                fixed4 col;
                //插值
                float lp = 0.0;
				// 如果当前像素位置处于 0 - _Pos1 的范围内，就使用color1与color2的插值计算颜色
				if (i.uv.y >= _Pos1)
				{
					// 插值 = 当前像素在pos1范围内的比重 = 在当前像素点的y值 / _Pos1界限值 
					lp = (1 - i.uv.y) / (1 - _Pos1);
					//像素输出的颜色 = _Color1 - _Color2之间，处于lp位置的颜色
					//lerp 三个参数为起始值，终止值，比重
					col = lerp(_Color1, _Color2, lp);
				}
				// 否则就使用第二个颜色
				else
				{
					col = _Color2;
				}
				//合成贴图、uv与颜色
                return tex2D(_MainTex, i.uv) * col;
            }
            ENDCG
        }
    }
}
