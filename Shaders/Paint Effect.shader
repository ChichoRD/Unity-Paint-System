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
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
			float4 worldPosition : TEXCOORD1;
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
		
		v2f vert(appdata v)
		{
			v2f o;
			o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
			
			o.uv = v.uv;
			float4 uv = float4(0, 0, 0, 1);
			uv.xy = (v.uv.xy * 2 - 1) * float2(1, _ProjectionParams.x);
			
			o.vertex = uv;
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
		
		ENDHLSL
		
		Pass
        {
		    Name "Paint Transparency Circle"
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			HLSLPROGRAM
			
			float4 frag(v2f i) : SV_Target
			{
				float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				
				float m = Mask(i.worldPosition.xyz, _PainterPosition, _Radius, _Hardness);
				float edge = m * _Strength;
				
				return lerp(c, _PainterColor, edge);
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
				float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				
				float m = Mask(i.worldPosition.xyz, _PainterPosition, _Radius, _Hardness);
				float edge = m * _Strength;
								
				return lerp(c, _PainterColor, edge);
			}
			
			ENDHLSL
        }

    }
}
