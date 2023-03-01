Shader "Hidden/Paint Effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
	    Tags 
        {
		    "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

		HLSLINCLUDE
		
		#pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
			float4 worldPosition : TEXCOORD1;
			float3 normal : NORMAL;
        };
		
        TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);
		float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
		
		float3 _PainterPosition;
		float _Radius;
		float _Hardness;
		float _Strength;
		float4 _PainterColor;

		TEXTURE2D(_PaintTex);
		SAMPLER(sampler_PaintTex);
		float3 _PaintTexRotation;
		float2 _PaintTexScale;
		float2 _PaintTexOffset;
		
		v2f vert(appdata v)
		{
			v2f o;
			o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
			
			o.uv = v.uv;
			float4 uv = float4(0, 0, 0, 1);
			uv.xy = (v.uv.xy * 2 - 1) * float2(1, _ProjectionParams.x);
			
			o.vertex = uv;

			o.normal = mul(unity_WorldToObject, v.normal);
			return o;
        }

		float taxicarDistance(float3 a, float3 b)
		{
			return abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z);
		}

		float Mask(float3 position, float3 center, float radius, float hardness)
		{
			float m = distance(position, center);
			//m = taxicarDistance(position, center);
			return 1 - smoothstep(radius * hardness, radius, m);
		}

		void RotateRadiansFloat(float2 UV, float2 Center, float Rotation, out float2 Out)
		{
			UV -= Center;
			float s = sin(Rotation);
			float c = cos(Rotation);
			float2x2 rMatrix = float2x2(c, -s, s, c);
			rMatrix *= 0.5;
			rMatrix += 0.5;
			rMatrix = rMatrix * 2 - 1;
			UV.xy = mul(UV.xy, rMatrix);
			UV += Center;
			Out = UV;
		}
		
		ENDHLSL
		
		Pass
        {
		    Name "Paint Transparency Circle"
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			HLSLPROGRAM
			
			float4 frag(v2f i) : SV_Target
			{
				float4 t = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				
				float m = Mask(i.worldPosition.xyz, _PainterPosition, _Radius, _Hardness);
				float edge = m * _Strength;
				
				return lerp(t, _PainterColor, edge);
			}
			
			ENDHLSL
        }
		
		Pass
        {
		    Name "Paint Transparency Texture"
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			HLSLPROGRAM
			
			float4 frag(v2f i) : SV_Target
			{
				#define FOUR_OVER_PI 1.27323954474
				float size = FOUR_OVER_PI / _Radius;

				float2 dxy = _PainterPosition.xy;
				float2 dyz = _PainterPosition.yz;
				float2 dxz = _PainterPosition.xz;

				float2 uv_front = i.worldPosition.xy - dxy;
				float2 uv_side = i.worldPosition.yz - dyz;
				float2 uv_top = i.worldPosition.xz - dxz;

				RotateRadiansFloat(uv_front, 0.0, _PaintTexRotation.z, uv_front);
				RotateRadiansFloat(uv_side, 0.0, _PaintTexRotation.x, uv_side);
				RotateRadiansFloat(uv_top, 0.0, _PaintTexRotation.y, uv_top);

				uv_front = (uv_front * size + 0.5) * _PaintTexScale + _PaintTexOffset;
				uv_side = (uv_side * size + 0.5) * _PaintTexScale + _PaintTexOffset;
				uv_top = (uv_top * size + 0.5) * _PaintTexScale + _PaintTexOffset;

				float3 weights = abs(i.normal);
				float3 signs = sign(weights);
				weights = pow(weights, 0.5);
				weights = (weights / (weights.x + weights.y + weights.z)) * signs;

                float4 paint0 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_front) * weights.z;
                float4 paint1 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_side) * weights.x;
                float4 paint2 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_top) * weights.y;

				float4 t = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				float4 p = paint0 + paint1 + paint2;

				float m = Mask(i.worldPosition.xyz, _PainterPosition, _Radius, _Hardness);
				float edge = m * _Strength;
								
				return lerp(t, _PainterColor, edge * p);
			}
			
			ENDHLSL
        }

    }
}
