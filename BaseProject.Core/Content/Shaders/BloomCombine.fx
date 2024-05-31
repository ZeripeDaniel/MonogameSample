// 픽셀 셰이더는 블룸 이미지와 원본 장면을 결합하여,
// 조절 가능한 강도 레벨과 채도를 사용합니다.
// 이것은 블룸 후처리를 적용하는 마지막 단계입니다.

sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;


// 색상의 채도를 조정하는 도우미 함수입니다.
float4 AdjustSaturation(float4 color, float saturation)
{
    // 상수 0.3, 0.59, 0.11은 인간의 눈이 녹색 빛에 더 민감하고
    // 파란색 빛에는 덜 민감하기 때문에 선택되었습니다.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // 블룸 이미지와 원본 기본 이미지 색상을 조회합니다.
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // 색상 채도와 강도를 조정합니다.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // 블룸이 많은 영역에서 기본 이미지를 어둡게 하여,
    // 과도하게 밝아 보이지 않도록 합니다.
    base *= (1 - saturate(bloom));
    
    // 두 이미지를 결합합니다.
    return base + bloom;
}


technique BloomCombine
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
