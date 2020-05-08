Shader "Custom/SeamBlendShader"
{
    // DIFFUSE TEXTURE

    Properties {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }
		LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert addshadow
		#pragma multi_compile __ HORIZON_WAVES 
		#pragma multi_compile __ BEND_ON

		// Global properties to be set by BendControllerRadial script
		uniform half3 _CurveOrigin;
		uniform fixed3 _ReferenceDirection;
		uniform half _Curvature;
		uniform fixed3 _Scale;
		uniform half _FlatMargin;
		uniform half _HorizonWaveFrequency;

        struct Input {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }

		half4 Bend(half4 v)
		{
			half4 wpos = mul(unity_ObjectToWorld, v);

			half2 xzDist = (wpos.xz - _CurveOrigin.xz) / _Scale.xz;
			half dist = length(xzDist);
			fixed waveMultiplier = 1;

			#if defined(HORIZON_WAVES)
			half2 direction = lerp(_ReferenceDirection.xz, xzDist, min(dist, 1));

			half theta = acos(clamp(dot(normalize(direction), _ReferenceDirection.xz), -1, 1));

			waveMultiplier = cos(theta * _HorizonWaveFrequency);
			#endif

			dist = max(0, dist - _FlatMargin);

			wpos.y -= dist * dist * _Curvature * waveMultiplier;

			wpos = mul(unity_WorldToObject, wpos);

			return wpos;
		}

		void vert(inout appdata_full v)
		{
			#if defined(BEND_ON)
			v.vertex = Bend(v.vertex);
			#endif
		}
        ENDCG
    }
    Fallback "Diffuse"
}
