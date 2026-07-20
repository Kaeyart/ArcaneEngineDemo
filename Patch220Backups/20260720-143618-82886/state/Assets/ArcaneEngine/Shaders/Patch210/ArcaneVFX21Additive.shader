Shader "Arcane/VFX21/Additive"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _GradientTex ("Gradient", 2D) = "white" {}
        _PrimaryColor ("Primary", Color) = (0.2,0.7,1,1)
        _SecondaryColor ("Secondary", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0,8)) = 1
        _Opacity ("Opacity", Range(0,1)) = 1
        _Pulse ("Pulse", Range(0,1)) = 0.5
        _Charge ("Charge", Range(0,1)) = 1
        _Dissolve ("Dissolve", Range(0,1)) = 0
        _NoiseScale ("Noise Scale", Float) = 3
        _NoiseSpeed ("Noise Speed", Float) = 1
        _FlowDirection ("Flow Direction", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off ZWrite Off Blend One One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float3 normal:TEXCOORD1; float3 view:TEXCOORD2; };
            sampler2D _MainTex; float4 _MainTex_ST;
            float4 _PrimaryColor, _SecondaryColor;
            float _Emission, _Opacity, _Pulse, _Charge, _Dissolve, _NoiseScale, _NoiseSpeed, _FlowDirection;
            float hash21(float2 p){ p=frac(p*float2(123.34,345.45)); p+=dot(p,p+34.345); return frac(p.x*p.y); }
            v2f vert(appdata v){ v2f o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=TRANSFORM_TEX(v.uv,_MainTex); o.normal=UnityObjectToWorldNormal(v.normal); float3 w=mul(unity_ObjectToWorld,v.vertex).xyz; o.view=_WorldSpaceCameraPos-w; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float2 flow=i.uv+float2(_Time.y*_NoiseSpeed*_FlowDirection*0.07,_Time.y*_NoiseSpeed*0.03);
                float noise=hash21(floor(flow*_NoiseScale*18)+floor(_Time.y*_NoiseSpeed));
                float mask=tex2D(_MainTex,i.uv).a;
                float fresnel=pow(1-saturate(dot(normalize(i.normal),normalize(i.view))),2.2);
                float edge=saturate(mask+fresnel*0.6+noise*0.15-_Dissolve);
                float3 color=lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,saturate(fresnel+_Pulse*0.45+noise*0.15));
                float alpha=edge*_Opacity*saturate(_Charge*0.65+0.35);
                return float4(color*_Emission*alpha,alpha);
            }
            ENDCG
        }
    }
    Fallback Off
}
