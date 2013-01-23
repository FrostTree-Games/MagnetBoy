sampler s0;

struct PixelShaderInput
{
	float2 Coords : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	// paul is not 100%, but coordinates are in [0, 1] units (may-beh)

	float4 base = tex2D(s0, input.Coords);
	float4 overlayer = input.Color;

	float4 tinted = float4(base.rgb * overlayer.rgb, base.a);

	return tinted;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
