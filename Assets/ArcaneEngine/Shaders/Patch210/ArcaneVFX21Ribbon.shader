Shader "Arcane/VFX21/Ribbon"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,8)) = 1.5
        _Opacity ("Opacity", Range(0,1)) = 0.9
        _FlowDirection ("Flow", Float) = 1
        _Pulse ("Pulse", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off ZWrite Off Blend One One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            sampler2D _MainTex; float4 _MainTex_ST,_PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_FlowDirection,_Pulse;
            v2f vert(appdata a){ v2f o; o.pos=UnityObjectToClipPos(a.vertex); o.uv=TRANSFORM_TEX(a.uv,_MainTex); o.color=a.color; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float flow=frac(i.uv.x*_FlowDirection-_Time.y*(0.7+_Pulse));
                float mask=tex2D(_MainTex,float2(flow,i.uv.y)).a;
                float edge=saturate(1-abs(i.uv.y*2-1));
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,flow);
                float a=mask*edge*_Opacity*i.color.a;
                return float4(c*_Emission*a,a);
            }
            ENDCG
        }
    }
    Fallback Off
}
