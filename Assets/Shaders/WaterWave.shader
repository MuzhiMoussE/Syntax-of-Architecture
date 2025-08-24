Shader "Book/WaterWave" 
{
    Properties 
    {
        _MainTex ("mainTex", 2D) = "white" {}
    }
 
    SubShader 
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Fog { Mode off }
 
            CGPROGRAM
 
            #pragma vertex vert_img // UnityCG.cginc中定义了vert_img方法, 对vertex和texcoord进行了处理, 输出v2f_img中的pos和uv
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float _A; 
            float _w1; 
            float _w2; 
            float _t; 
            float2 _o; 
            float _waveDist; 
            float _waveWidth; 
 
            fixed4 frag(v2f_img i) : SV_Target 
            {
                float2 vec = i.uv - _o.xy;
                vec.x *= _ScreenParams.x / _ScreenParams.y; // 按照屏幕长宽比进行缩放
                float radius = length(vec); 
                float leng = abs(radius - _waveDist);
                float offset = 0;
                if (leng < _waveWidth)
                {
                    offset = _A *sin(_w1 * radius - _w2 * _t) * (1 - leng / _waveWidth)/(_w1 * radius - _w2 * _t);
                }
                return tex2D(_MainTex, i.uv + offset * 0.707); // offset是一维的, uv是二维的, 需要除以根号2, 即乘以0.707
            }
 
            ENDCG
        }
    }
 
    Fallback off
}