Shader "Arcane/VFX21/Premultiplied"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,6)) = 1
        _Opacity ("Opacity", Range(0,1)) = 0.7
        _Pulse ("Pulse", Range(0,1)) = 0.5
        _Fracture ("Fracture", Range(0,1)) = 0
        _Dissolve ("Dissolve", Range(0,1)) = 0
        _Compression ("Compression", Range(-1,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Back ZWrite Off Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float3 n:TEXCOORD0; float3 v:TEXCOORD1; float2 uv:TEXCOORD2; };
            sampler2D _MainTex; float4 _PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_Pulse,_Fracture,_Dissolve,_Compression;
            float hash31(float3 p){ p=frac(p*0.1031); p+=dot(p,p.yzx+33.33); return frac((p.x+p.y)*p.z); }
            v2f vert(appdata a){ v2f o; float3 displaced=a.vertex.xyz+a.normal*_Compression*0.035; o.pos=UnityObjectToClipPos(float4(displaced,1)); o.n=UnityObjectToWorldNormal(a.normal); float3 w=mul(unity_ObjectToWorld,float4(displaced,1)).xyz; o.v=_WorldSpaceCameraPos-w; o.uv=a.uv; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float fres=pow(1-saturate(dot(normalize(i.n),normalize(i.v))),2.5);
                float fracture=step(0.84,hash31(float3(floor(i.uv*18),floor(_Time.y*2))))*_Fracture;
                float alpha=saturate(fres*0.75+0.18+fracture*0.5-_Dissolve)*_Opacity;
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(fres+fracture+_Pulse*0.15));
                return float4(c*(_Emission*alpha),alpha);
            }
            ENDCG
        }
    }
    Fallback Off
}
