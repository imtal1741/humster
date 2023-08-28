Shader "Custom/WorldSpaceTexture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}

		_TextureScale("TextureScale", Range(0,10)) = 1

		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
 
        CGPROGRAM
		#pragma surface surf WrapLambert
 
        sampler2D _MainTex;
		float _TextureScale;
		float4 _AmbientColor;

		half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			float lightIntensity = NdotL > 0.5 ? 1 : 1.5;
			half diff = (NdotL * 0.5 + 0.5) * lightIntensity;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
			c.a = s.Alpha;
			return c;
		}
 
        struct Input {
            float3 worldNormal;
            float3 worldPos;
        };
 
        void surf (Input IN, inout SurfaceOutput o)
		{
 
            if(abs(IN.worldNormal.y) > 0.5)
            {
                o.Albedo = tex2D(_MainTex, IN.worldPos.xz * _TextureScale);
            }
            else if(abs(IN.worldNormal.x) > 0.5)
            {
                o.Albedo = tex2D(_MainTex, IN.worldPos.zy * _TextureScale);
            }
            else
            {
                o.Albedo = tex2D(_MainTex, IN.worldPos.xy * _TextureScale);
            }

            o.Albedo *= _AmbientColor;
        }
 
        ENDCG
    }
    FallBack "Diffuse"
}