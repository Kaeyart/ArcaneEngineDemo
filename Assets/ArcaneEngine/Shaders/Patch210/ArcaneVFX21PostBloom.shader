Shader "Arcane/VFX21/PostBloom"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _BloomEnabled ("Bloom Enabled", Float) = 1
        _BloomStrength ("Bloom Strength", Range(0,1)) = 0.25
        _Threshold ("Threshold", Range(0,2)) = 0.75
        _Flash ("Flash", Range(0,1)) = 0
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_TexelSize; float _BloomEnabled,_BloomStrength,_Threshold,_Flash; float4 _FlashColor;
            fixed4 frag(v2f_img i):SV_Target
            {
                float3 src=tex2D(_MainTex,i.uv).rgb;
                float2 t=_MainTex_TexelSize.xy*1.5;
                float3 blur=0;
                blur+=tex2D(_MainTex,i.uv+float2(t.x,0)).rgb;
                blur+=tex2D(_MainTex,i.uv-float2(t.x,0)).rgb;
                blur+=tex2D(_MainTex,i.uv+float2(0,t.y)).rgb;
                blur+=tex2D(_MainTex,i.uv-float2(0,t.y)).rgb;
                blur+=tex2D(_MainTex,i.uv+float2(t.x,t.y)).rgb;
                blur+=tex2D(_MainTex,i.uv-float2(t.x,t.y)).rgb;
                blur/=6;
                float luminance=dot(blur,float3(0.2126,0.7152,0.0722));
                float3 bloom=blur*saturate((luminance-_Threshold)*3)*_BloomStrength*_BloomEnabled;
                float3 result=src+bloom;
                result=lerp(result,_FlashColor.rgb,saturate(_Flash)*0.65);
                return float4(result,1);
            }
            ENDCG
        }
    }
    Fallback Off
}
