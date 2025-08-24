Shader "Book/WaterFall" {
	Properties {
    _MainTint ("Diffuse Tint", Color) = (1, 1, 1, 1)
    _InputColor ("Input Color", Color) = (1, 1, 1, 1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _ScrollXSpeed ("X Scroll Speed", Range(-10, 10)) = 2
    _ScrollYSpeed ("Y Scroll Speed", Range(-10, 10)) = 2
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows
    #pragma target 3.0

    fixed4 _MainTint;
    fixed4 _InputColor;
    fixed _ScrollXSpeed;
    fixed _ScrollYSpeed;
    sampler2D _MainTex;

    struct Input {
        float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutputStandard o) 
    {
        fixed2 scrolledUV = IN.uv_MainTex;

        fixed xScrollValue = _ScrollXSpeed * _Time;
        fixed yScrollValue = _ScrollYSpeed * _Time;

        scrolledUV += fixed2(xScrollValue, yScrollValue);

        half4 texColor = tex2D(_MainTex, scrolledUV); // 获取贴图颜色

        // 输入颜色与贴图颜色叠加
        half4 finalColor = _InputColor * texColor;

        o.Albedo = finalColor.rgb;
        o.Alpha = finalColor.a;
    }
    ENDCG
}

FallBack "Diffuse"
}