sampler s0;

float screenWidth = 720;
float screenHeight = 480;

//between 0.0f and 1.0f
float time = 1.0f;

float2 screenCenter = float2(360, 240);

struct PixelShaderInput
{
	float2 Coords : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	// paul is not 100%, but coordinates are in [0, 1] units (may-beh)
	float posX = (screenWidth * input.Coords.x);
	float posY = (screenHeight * input.Coords.y);

	float tileCenterX = (floor(posX / 16) * 16) + 8;
	float tileCenterY = (floor(posY / 16) * 16) + 8;

	if (sqrt(pow(posX - tileCenterX,2) + pow(posY - tileCenterY,2))/12 > (1.4775 * time - (sqrt(pow(posX - (screenWidth/2),2) + pow(posY - (screenHeight/2),2)))/720))
	{
		float4 base = tex2D(s0, input.Coords);
		float4 overlayer = input.Color;

		return float4(base.rgb * overlayer.rgb, base.a);
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
