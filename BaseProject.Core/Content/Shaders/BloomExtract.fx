// 픽셀 셰이더는 이미지의 더 밝은 영역을 추출합니다.
// 이것은 블룸 후처리를 적용하는 첫 번째 단계입니다.

sampler TextureSampler : register(s0);

float BloomThreshold;


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // 원본 이미지 색상을 조회합니다.
    float4 c = tex2D(TextureSampler, texCoord);

    // 지정된 임계값보다 밝은 값만 유지하도록 조정합니다.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
#if SM4		
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
#else
        PixelShader = compile ps_2_0 PixelShaderFunction();
#endif
    }
}
