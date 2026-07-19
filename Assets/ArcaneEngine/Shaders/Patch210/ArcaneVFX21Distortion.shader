Shader "Arcane/VFX21/Distortion"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.4,0.05,0.8,1)
        _SecondaryColor ("Secondary", Color) = (0.9,0.3,1,1)
        _Opacity ("Opacity", Range(0,1)) = 0.55
        _Emission ("Emission", Range(0,5)) = 1
        _DistortionStrength ("Distortion", Range(0,0.08)) = 0.018
        _Pulse ("Pulse", Range(0,1)) = 0.5
        _Dissolve ("Dissolve", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent+20" "RenderType"="Transparent" }
        GrabPass { "_ArcaneVFX21Grab" }
        Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float4 grab:TEXCOORD0; float2 uv:TEXCOORD1; float3 n:TEXCOORD2; float3 v:TEXCOORD3; };
            sampler2D _ArcaneVFX21Grab,_MainTex; float4 _PrimaryColor,_SecondaryColor; float _Opacity,_Emission,_DistortionStrength,_Pulse,_Dissolve;
            v2f vert(appdata a){ v2f o; o.pos=UnityObjectToClipPos(a.vertex); o.grab=ComputeGrabScreenPos(o.pos); o.uv=a.uv; o.n=UnityObjectToWorldNormal(a.normal); float3 w=mul(unity_ObjectToWorld,a.vertex).xyz; o.v=_WorldSpaceCameraPos-w; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float2 wave=float2(sin(i.uv.y*25+_Time.y*3),cos(i.uv.x*21-_Time.y*2))*_DistortionStrength*(0.4+_Pulse);
                float4 grab=i.grab; grab.xy+=wave*grab.w;
                float3 scene=tex2Dproj(_ArcaneVFX21Grab,UNITY_PROJ_COORD(grab)).rgb;
                float fres=pow(1-saturate(dot(normalize(i.n),normalize(i.v))),2.2);
                float alpha=saturate(fres+0.18-_Dissolve)*_Opacity;
                float3 edge=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,fres)*_Emission;
                return float4(lerp(scene,edge,saturate(fres*0.75)),alpha);
            }
            ENDCG
        }
    }
    Fallback "Arcane/VFX21/Premultiplied"
}
