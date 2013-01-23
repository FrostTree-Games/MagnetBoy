sampler s0;

int screenWidth = 720;
int screenHeight = 480;

float spread = 1.0;

struct PixelShaderInput
{
	float2 Coords : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float modX = ((screenWidth * input.Coords.x) / 16) % 2;
	float modY = ((screenHeight * input.Coords.y) / 16) % 2;

	if (modX <= 1 && modY > 1.0 && modX < spread)
	{
		return float4(0, 0, 0, 0.125);
	}
	else if (modX > 1 && modY <= 1.0 && modX < 1 + spread)
	{
		return float4(0, 0, 0, 0.125);
	}
	else 
	{
		return float4(0, 0, 0, 0);
	}
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
