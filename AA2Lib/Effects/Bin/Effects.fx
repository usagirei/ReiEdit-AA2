float Factor : register(c0);
sampler2D Samp : register(S0);
sampler2D Samp2 : register(S1);

float3 rgb_to_hsv_no_clip(float3 RGB)
{
	float3 HSV;
	
	float minChannel, maxChannel;
	if (RGB.x > RGB.y) {
		maxChannel = RGB.x;
		minChannel = RGB.y;
	} else {
		maxChannel = RGB.y;
		minChannel = RGB.x;
	}
 
	if (RGB.z > maxChannel) maxChannel = RGB.z;
	if (RGB.z < minChannel) minChannel = RGB.z;
	
	HSV.xy = 0;
	HSV.z = maxChannel;
	float delta = maxChannel - minChannel;
	if (delta != 0) { 
	   HSV.y = delta / HSV.z;
	   float3 delRGB;
	   delRGB = (HSV.zzz - RGB + 3*delta) / (6.0*delta);
	   if      ( RGB.x == HSV.z ) HSV.x = delRGB.z - delRGB.y;
	   else if ( RGB.y == HSV.z ) HSV.x = ( 1.0/3.0) + delRGB.x - delRGB.z;
	   else if ( RGB.z == HSV.z ) HSV.x = ( 2.0/3.0) + delRGB.y - delRGB.x;
	}
	return (HSV);
}

float3 hsv_to_rgb(float3 HSV)
{
	float3 RGB;// = HSV.z;

	float var_h = HSV.x * 6;
	float var_i = floor(var_h);  
	float var_1 = HSV.z * (1.0 - HSV.y);
	float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
	float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));

	if (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
	else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
	else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
	else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
	else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
	else { RGB = float3(HSV.z, var_1, var_2); }

	return (RGB);
}

float4 hueShift(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(Samp, uv);
	float3 hsv = rgb_to_hsv_no_clip(col.xyz);
	hsv.x += Factor;

	if (hsv.x > 1.0) { hsv.x -= 1.0; }
	return float4(hsv_to_rgb(hsv), col.w);
}

float4 valShift(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(Samp, uv);
	float aFactor = abs(Factor);
	if (Factor > 0){
		float dR = 1 - col.x;
		float dG = 1 - col.y;
		float dB = 1 - col.z;
		col.x += dR * aFactor;
		col.y += dG * aFactor;
		col.z += dB * aFactor;
	}
	if (Factor < 0){
		float dR = col.x;
		float dG = col.y;
		float dB = col.z;
		col.x -= dR * aFactor;
		col.y -= dG * aFactor;
		col.z -= dB * aFactor;
	}
	return float4(col.xyz, col.w);
}

float4 multiply(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(Samp, uv);
	float4 col2 = tex2D(Samp2, uv);
	float3 colOut;
	float alpha = col2.w;
	float ialpha = 1 - alpha;
	colOut.x = (col.x * ialpha) + (col.x * col2.x * alpha);
	colOut.y = (col.y * ialpha) + (col.y * col2.y * alpha);
	colOut.z = (col.z * ialpha) + (col.z * col2.z * alpha);
	return float4(colOut.xyz, col.w);
}
