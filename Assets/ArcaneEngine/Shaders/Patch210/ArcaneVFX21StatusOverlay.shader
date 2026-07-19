Shader "Arcane/VFX21/StatusOverlay"
{
    Properties
    {
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,5)) = 1
        _Opacity ("Opacity", Range(0,1)) = 0.5
        _Pulse ("Pulse", Range(0,1)) = 0.5
        _Fracture ("Fracture", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent+10" "RenderType"="Transparent" }
        Cull Back ZWrite Off Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; };
            struct v2f { float4 pos:SV_POSITION; float3 n:TEXCOORD0; float3 v:TEXCOORD1; float3 w:TEXCOORD2; };
            float4 _PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_Pulse,_Fracture;
            v2f vert(appdata a){ v2f o; float3 p=a.vertex.xyz+a.normal*0.012; o.pos=UnityObjectToClipPos(float4(p,1)); o.n=UnityObjectToWorldNormal(a.normal); o.w=mul(unity_ObjectToWorld,float4(p,1)).xyz; o.v=_WorldSpaceCameraPos-o.w; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float fres=pow(1-saturate(dot(normalize(i.n),normalize(i.v))),3);
                float pattern=step(0.82,frac(sin(dot(floor(i.w*12),float3(12.9,78.2,35.7)))*43758.5))*_Fracture;
                float a=saturate(fres*0.8+pattern*0.6)*_Opacity;
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(fres+pattern+_Pulse*0.15));
                return float4(c*_Emission*a,a);
            }
            ENDCG
        }
    }
    Fallback "Arcane/VFX21/Premultiplied"
}
