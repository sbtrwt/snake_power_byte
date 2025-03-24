Shader "Custom/BulletFire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 1.0
        _TrailLength ("Trail Length", Float) = 0.5
        _MuzzleFlash ("Muzzle Flash", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GlowIntensity;
            float _TrailLength;
            float _MuzzleFlash;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Muzzle flash effect
                float muzzleFlash = smoothstep(0.0, 1.0, _MuzzleFlash - i.uv.x);
                fixed4 muzzleColor = fixed4(1.0, 0.5, 0.0, 1.0) * muzzleFlash;

                // Trail effect
                float trail = smoothstep(0.0, _TrailLength, i.uv.x);
                fixed4 trailColor = _Color * trail;

                // Glow effect
                float glow = _GlowIntensity * (1.0 - i.uv.x);
                fixed4 glowColor = _Color * glow;

                // Combine effects
                fixed4 finalColor = texColor * (_Color + muzzleColor + trailColor + glowColor);
                finalColor.a = texColor.a; // Preserve texture alpha

                return finalColor;
            }
            ENDCG
        }
    }
}