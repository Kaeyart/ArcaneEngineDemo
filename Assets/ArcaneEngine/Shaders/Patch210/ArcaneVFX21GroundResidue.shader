Shader "Arcane/VFX21/GroundResidue"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,4)) = 0.7
        _Opacity ("Opacity", Range(0,1)) = 0.65
        _Pulse ("Pulse", Range(0,1)) = 0.5
        _Dissolve ("Dissolve", Range(0,1)) = 0
        _NoiseScale ("Noise Scale", Float) = 3
        _NoiseSpeed ("Noise Speed", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent-5" "RenderType"="Transparent" }
        Cull Off ZWrite Off Offset -1,-1 Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            sampler2D _MainTex; float4 _PrimaryColor,_SecondaryColor; float _Emission,_Opacity,_Pulse,_Dissolve,_NoiseScale,_NoiseSpeed;
            float hash(float2 p){ return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453); }
            v2f vert(appdata a){ v2f o; o.pos=UnityObjectToClipPos(a.vertex); o.uv=a.uv; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float2 p=i.uv*2-1; float d=length(p); float radial=saturate(1-d);
                float noise=hash(floor((i.uv+_Time.y*_NoiseSpeed*0.02)*_NoiseScale*20));
                float edge=smoothstep(0.02,0.14,radial)*smoothstep(0.01,0.12,1-_Dissolve+noise*0.15);
                float3 c=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(noise*0.4+_Pulse*0.3));
                return float4(c*(0.35+_Emission*0.4),edge*_Opacity);
            }
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}
