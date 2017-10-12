///  <summary>Input Brush</summary>
sampler2D inputBrush : register(s0);

/// <summary>Displacement Brush</summary>
sampler2D displaceBrush : register(s1);

/// <summary>Light Direction</summary>
/// <minValue>-1,-1</minValue>
/// <maxValue>1,1</maxValue>
/// <defaultValue>1,1</defaultValue>
float2 lightPos : register(C0);

///  <maxValue>5 </maxValue>
///  <minValue>0 </minValue>
///  <defaultValue>1 </defaultValue>
float lightHeight : register(c1);

/// <summary>Displacement Amount</summary>
/// <minValue> -0.1</minValue>
/// <maxValue>0.1</maxValue>
/// <defaultValue>0.015</defaultValue>
float displacementAmount : register(C2);

///  <summary>Shadow</summary>
///  <defaultValue>#FF5ABEC8</defaultValue>
float4 shadowColor : register(c3);

///  <summary>Light Color</summary>
///  <defaultValue>#7FFFFFFF</defaultValue>
float4 lightColor : register(c5);

///  <summary>Glow Color </summary>
///  <defaultValue>#7FFFFFFF </defaultValue>
float4 glowColor: register(c6);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	// Fixed Camera Vector
	const float3 camera = float3(0,0,1);
	
	// Sample Maps
	float4 normalMap = tex2D(displaceBrush, uv.xy);

	// Create Light Direction Vector
	float3 lightDir = float3(-lightPos, lightHeight);
	
	// Adjust Normals to -1 < x < 1
	float3 normals = (normalMap.rgb * 2) - 1;
	normals.y *= -1;
	  
	// Calculate Displacement
	float2 displacement = (float2(normals.x, normals.y) * -normals.z * displacementAmount);
	float2 displaced = uv.xy + displacement;

	// Read Input Displaced, or Transparent if xy < 0
	float4 input;
	input = tex2D(inputBrush, frac(displaced)); 
		
	// Calculate Diffuse Lighting
	float diffuse = dot(normals, lightDir);
	diffuse = saturate(pow(diffuse,5));
	
	//return float4(diffuse.xxx,1);
	// Map Output to Color
	float4 output = float4(input.rgb,input.a);
	
	// Light
	// --Map from Black to Color using Diffuse and Blend from Black to Result using Alpha
	output.rgb *= ((lightColor.rgb * diffuse) * (1-lightColor.a)) + (lightColor.rgb * lightColor.a);
	
	// Shadow
	// --Map from White to Color Using Alpha and Blend from White to Result using Diffuse
	output.rgb *= (shadowColor.rgb * shadowColor.a + (1-shadowColor.a)) * (1 - diffuse) + diffuse;
	

	output.rgb += (1-dot(normals,camera)) * glowColor.rgb * glowColor.a;
	
	//output.rgb = diffuse.xxx;
	return output;
}
