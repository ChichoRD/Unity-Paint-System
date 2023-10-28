Shader "Hidden/Paint Composite"
{
    Properties
    {
        _MaskTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
	    Tags 
        {
		    "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

		HLSLINCLUDE
		
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
		
        uniform TEXTURE2D(_MaskTex);
		uniform SAMPLER(sampler_MaskTex);

		uniform float3 _PainterPosition;
		uniform float _Radius;
		uniform float _Hardness;
		uniform float _Strength;
		uniform float4 _PainterColor;

		uniform TEXTURE2D(_PaintTex);
		uniform SAMPLER(sampler_PaintTex);
		uniform float3 _PaintTexRotation;
		uniform float2 _PaintTexTiling;
		uniform float2 _PaintTexOffset;
		
		v2f vert(appdata v)
		{
			v2f o;
			o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
			
			o.uv = v.uv;
			float4 uv = float4(0, 0, 0, 1);
			uv.xy = (v.uv.xy * 2 - 1) * float2(1, _ProjectionParams.x);
			
			o.vertex = uv;

			float3 normal = mul(v.normal, (float3x3)unity_WorldToObject);
			o.normal = normalize(normal);
			return o;
        }

		float SDSphere(float3 p, float3 centre, float radius)
		{
			return distance(p, centre) - radius;
		}
		
		float SDBox(float3 p, float3 centre, float3 halfExtents)
		{
			float3 d = abs(p - centre) - halfExtents;
			return min(max(d.x, max(d.y, d.z)), 0.0f) + length(max(d, 0.0f));
		}

		float DrawSDFAntialiasedContour(float d)
		{
			const float thickness = 0.1;
			const float smoothness = 0.01;
			
			return smoothstep(1.0f - thickness - smoothness,
							  1.0f - thickness + smoothness,
							  d);
		}

		float ApplyHardness(float d, float hardness, float radius)
		{
			return smoothstep(radius * hardness, radius, d);
		}

		float ApplyStrength(float d, float strength)
		{
			return (1.0f - d) * strength;
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

			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag(v2f i) : SV_Target
			{
				float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
				
				float edge = ApplyStrength(
								 ApplyHardness(
									 SDSphere(i.worldPosition.xyz, _PainterPosition, _Radius),
									 _Hardness, _Radius),
								 _Strength);
				
				return lerp(mask, _PainterColor, edge);
			}
			
			ENDHLSL
        }
		
		Pass
        {
		    Name "Paint Transparency Texture"
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			float4 frag(v2f i) : SV_Target
			{
				float2 dxy = _PainterPosition.xy;
				float2 dyz = _PainterPosition.yz;
				float2 dxz = _PainterPosition.xz;

				float2 uv_front = i.worldPosition.xy - dxy;
				float2 uv_side = i.worldPosition.yz - dyz;
				float2 uv_top = i.worldPosition.xz - dxz;

				RotateRadiansFloat(uv_front, 0.0f, _PaintTexRotation.z, uv_front);
				RotateRadiansFloat(uv_side, 0.0f, _PaintTexRotation.x, uv_side);
				RotateRadiansFloat(uv_top, 0.0f, _PaintTexRotation.y, uv_top);

				uv_front	= ((uv_front	/ _Radius * 0.5f * _PaintTexTiling) * 0.5f + 0.5f + _PaintTexOffset);
				uv_side		= ((uv_side		/ _Radius * 0.5f * _PaintTexTiling) * 0.5f + 0.5f + _PaintTexOffset);
				uv_top		= ((uv_top		/ _Radius * 0.5f * _PaintTexTiling) * 0.5f + 0.5f + _PaintTexOffset);

				float3 weights = abs(i.normal);
				float3 signs = sign(weights);
				weights = pow(weights, 0.5);
				weights = (weights / (weights.x + weights.y + weights.z)) * signs;

                float4 paint0 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_front) * weights.z;
                float4 paint1 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_side) * weights.x;
                float4 paint2 = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, uv_top) * weights.y;

				float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
				float4 p = paint0 + paint1 + paint2;

				float edge = ApplyStrength(
								 ApplyHardness(
									 SDBox(i.worldPosition.xyz, _PainterPosition, _Radius),
									 _Hardness, _Radius),
								 _Strength);
								 
				return lerp(mask, _PainterColor, edge * p);
			}
			
			ENDHLSL
        }
    }
}