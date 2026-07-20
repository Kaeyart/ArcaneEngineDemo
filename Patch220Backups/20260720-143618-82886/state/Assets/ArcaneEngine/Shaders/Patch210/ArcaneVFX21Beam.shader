Shader "Arcane/VFX21/Beam"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,10)) = 2
        _Opacity ("Opacity", Range(0,1)) = 0.9
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
            float4 _PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_Pulse;
            v2f vert(appdata a){ v2f o; o.pos=UnityObjectToClipPos(a.vertex); o.uv=a.uv; o.color=a.color; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float core=pow(saturate(1-abs(i.uv.y*2-1)),2.2);
                float flow=0.5+0.5*sin((i.uv.x-_Time.y*2.5)*20+_Pulse*6.28);
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(core+flow*0.25));
                float a=core*_Opacity*i.color.a;
                return float4(c*_Emission*a,a);
            }
            ENDCG
        }
    }
    Fallback "Arcane/VFX21/Ribbon"
}
