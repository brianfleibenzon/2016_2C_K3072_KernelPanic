// ---------------------------------------------------------
// Sombras en el image space con la tecnica de Shadows Map
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

#define SMAP_SIZE 1024
#define EPSILON 0.05f

float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3   g_vLightPos;  // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3   g_vLightDir;  // Direcion de la luz (en World Space) = normal al patch Bj

texture  g_txShadow;	// textura para el shadow map
sampler2D g_samShadow =
sampler_state
{
	Texture = <g_txShadow>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialDiffuseColor; //Color RGB

//Variables de las luces
int cantidadLuces;
float3 lightColor[4]; //Color RGB de las luces
float4 lightPosition[4]; //Posicion de las luces
float lightIntensity[4]; //Intensidad de las luces
float lightAttenuation[4]; //Factor de atenuacion de las luces

//Parametros de Spot		
float3 spotLightDir; //Direccion del cono de luz		
float spotLightAngleCos; //Angulo de apertura del cono de luz (en radianes)		
float spotLightExponent; //Exponente de atenuacion dentro del cono de luz		



//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float3 Norm :			TEXCOORD1;		// Normales
	float3 Pos :   			TEXCOORD2;		// Posicion real 3d
};

//-----------------------------------------------------------------------------
// Vertex Shader que implementa un shadow map
//-----------------------------------------------------------------------------
void VertShadow(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Depth : TEXCOORD0)
{
	// transformacion estandard
	oPos = mul(Pos, matWorld);					// uso el del mesh
	oPos = mul(oPos, g_mViewLightProj);		// pero visto desde la pos. de la luz

	// devuelvo: profundidad = z/w
	Depth.xy = oPos.zw;
}

//-----------------------------------------------------------------------------
// Pixel Shader para el shadow map, dibuja la "profundidad"
//-----------------------------------------------------------------------------
void PixShadow(float2 Depth : TEXCOORD0, out float4 Color : COLOR)
{
	// parche para ver el shadow map
	//float k = Depth.x/Depth.y;
	//Color = (1-k);
	Color = Depth.x / Depth.y;
}

technique RenderShadow
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertShadow();
		PixelShader = compile ps_3_0 PixShadow();
	}
}

//-----------------------------------------------------------------------------
// Vertex Shader para dibujar la escena pp dicha con sombras
//-----------------------------------------------------------------------------
void VertScene(float4 iPos : POSITION,
	float2 iTex : TEXCOORD0,
	float3 iNormal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Tex : TEXCOORD0,
	out float4 vPos : TEXCOORD1,
	out float3 vNormal : TEXCOORD2,
	out float4 vPosLight : TEXCOORD3,
	out float3 oWorldNormal : TEXCOORD4,
	out float3 oSpotLightVec : TEXCOORD5,
	out float3 oSpotHalfAngleVec : TEXCOORD6
)
{
	// transformo al screen space
	oPos = mul(iPos, matWorldViewProj);

	// propago coordenadas de textura
	Tex = iTex;

	// propago la normal
	vNormal = mul(iNormal, (float3x3)matWorldView);

	// propago la posicion del vertice en World space
	vPos = mul(iPos, matWorld);
	// propago la posicion del vertice en el espacio de proyeccion de la luz
	vPosLight = mul(vPos, g_mViewLightProj);

	oWorldNormal = mul(iNormal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	oSpotLightVec = lightPosition[0].xyz - vPos;

	//ViewVec (V): vector que va desde el vertice hacia la camara.			//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = lightPosition[0].xyz - vPos;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular			//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	oSpotHalfAngleVec = viewVector + oSpotLightVec;

}

//-----------------------------------------------------------------------------
// Pixel Shader para dibujar la escena
//-----------------------------------------------------------------------------
//Funcion para calcular color RGB de Diffuse

float3 computeDiffuseComponent(float3 surfacePosition, float3 N, int i)
{
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(lightPosition[i].xyz - surfacePosition);

	float3 Ln = (lightPosition[i].xyz - surfacePosition) / distAtten;
	distAtten = distAtten * lightAttenuation[i];
	float intensity = lightIntensity[i] / distAtten; //Dividimos intensidad sobre distancia

													 //Calcular Diffuse (N dot L)
	return intensity * lightColor[i].rgb * materialDiffuseColor * max(1.0, dot(N, Ln));
}



float4 PixScene(float2 Tex : TEXCOORD0,
	float4 vPos : TEXCOORD1,
	float3 vNormal : TEXCOORD2,
	float4 vPosLight : TEXCOORD3,
	float4 iWorldNormal : TEXCOORD4,
	float3 iSpotLightVec : TEXCOORD5,
	float3 iSpotHalfAngleVec	: TEXCOORD6
) :COLOR
{
	float3 vLight = normalize(float3(vPos - g_vLightPos));
	float cono = dot(vLight, g_vLightDir);
	float4 K = 0.0;
	if (cono > 0.7)
	{
		// coordenada de textura CT
		float2 CT = 0.5 * vPosLight.xy / vPosLight.w + float2(0.5, 0.5);
		CT.y = 1.0f - CT.y;

		// sin ningun aa. conviene con smap size >= 512
		float I = (tex2D(g_samShadow, CT) + EPSILON < vPosLight.z / vPosLight.w) ? 0.5f : 1.0f;

		if (cono < 0.8)
			I *= 1 - (0.8 - cono) * 10;

		K = I;
	}

	float3 Nn = normalize(iWorldNormal);
	float3 Ln = normalize(iSpotLightVec);
	float3 Hn = normalize(iSpotHalfAngleVec);

	// Calcular atenuacion por Spot Light.Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, Ln);
	spotAtten = (spotAtten > spotLightAngleCos)
		? pow(spotAtten, spotLightExponent)
		: 0.0;

	//Emissive + Diffuse de 4 luces PointLight
	float3 diffuseLighting = materialEmissiveColor;
	
	diffuseLighting += computeDiffuseComponent(vPos, Nn, 0) * spotAtten;

	for (int i = 1; i < cantidadLuces; i++)
		diffuseLighting += computeDiffuseComponent(vPos, Nn, i);		


	float4 color_base = tex2D(diffuseMap, Tex);
	color_base.rgb *= 0.5*K * diffuseLighting;
	return color_base;
}

technique RenderScene
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertScene();
		PixelShader = compile ps_3_0 PixScene();
	}
}