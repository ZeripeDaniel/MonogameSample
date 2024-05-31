// 픽셀 셰이더는 1차원 가우시안 블러 필터를 적용합니다.
// 이는 블룸 후처리에서 두 번 사용됩니다. 
// 첫 번째는 수평 블러를 위해, 두 번째는 수직 블러를 위해 사용됩니다.

sampler TextureSampler : register(s0);

#define SAMPLE_COUNT 15 // 샘플 개수 정의

float2 SampleOffsets[SAMPLE_COUNT]; // 샘플 오프셋 배열
float SampleWeights[SAMPLE_COUNT]; // 샘플 가중치 배열


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    // 여러 개의 가중치가 적용된 이미지 필터 탭을 결합합니다.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];
    }
    
    return c; // 결합된 결과 반환
}


technique GaussianBlur
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
