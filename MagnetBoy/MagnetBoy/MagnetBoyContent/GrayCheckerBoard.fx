sampler s0;

int screenWidth = 720;
int screenHeight = 480;

float slideX = 0.0f;
float slideY = 0.0f;

struct PixelShaderInput
{
	float2 Coords : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float modX = ( abs(((screenWidth * input.Coords.x)) % screenWidth + slideX) / 16) % 2;
	float modY = ( abs(((screenHeight * input.Coords.y)) % screenHeight + slideY) / 16) % 2;

	if (modX <= 1 && modY > 1.0)
	{
		return float4(0, 0, 0, 0.125);
	}
	else if (modX > 1 && modY <= 1.0)
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
