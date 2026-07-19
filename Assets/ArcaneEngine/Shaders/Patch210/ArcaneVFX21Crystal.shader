Shader "Arcane/VFX21/Crystal"
{
    Properties
    {
        _PrimaryColor ("Primary", Color) = (0.2,0.8,1,1)
        _SecondaryColor ("Secondary", Color) = (0.9,1,1,1)
        _Emission ("Emission", Range(0,5)) = 1
        _Opacity ("Opacity", Range(0,1)) = 0.75
        _Fracture ("Fracture", Range(0,1)) = 0
        _Dissolve ("Dissolve", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Back ZWrite On Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float3 n:TEXCOORD0; float3 v:TEXCOORD1; float2 uv:TEXCOORD2; };
            float4 _PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_Fracture,_Dissolve;
            v2f vert(appdata a){ v2f o; o.pos=UnityObjectToClipPos(a.vertex); o.n=UnityObjectToWorldNormal(a.normal); float3 w=mul(unity_ObjectToWorld,a.vertex).xyz; o.v=_WorldSpaceCameraPos-w; o.uv=a.uv; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float fres=pow(1-saturate(dot(normalize(i.n),normalize(i.v))),2);
                float facet=pow(abs(dot(normalize(i.n),normalize(float3(0.4,0.8,0.2)))),3);
                float crack=step(0.92,frac(sin(dot(floor(i.uv*22),float2(12.9898,78.233)))*43758.5453))*_Fracture;
                float alpha=saturate(0.28+fres*0.55+facet*0.25+crack-_Dissolve)*_Opacity;
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(fres+facet+crack));
                return float4(c*(0.55+_Emission*0.35),alpha);
            }
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}
