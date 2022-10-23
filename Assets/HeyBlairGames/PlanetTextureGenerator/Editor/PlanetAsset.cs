using UnityEngine;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetAsset : ScriptableObject
	{
		public int			seedSurface;
		public int			seedLand;
		public int			seedLandColour01;
		public int			seedLandColour23;
		public int			seedCity;
		public int			seedCloud;

		public PlanetNoise	surfaceNoise;

		public PlanetNoise	landNoise;
		public Color		landColour0;
		public Color		landColour1;
		public Color		landColour2;
		public Color		landColour3;
		public PlanetNoise	landColourNoise01;
		public PlanetNoise	landColourNoise23;

		public Color		waterColour0;
		public Color		waterColour1;
		public float		waterLevel;
		public float		waterSpecular;
		public float		waterFalloff;

		public Color		iceColour;
		public float		iceReach;
		public float		iceHeight;

		public float		shadowRange;
		public float		shadowStrength;

		public float		cityReach;
		public float		cityHeight;
		public Color		cityColour;
		public int			cityCount;
		public int			cityMultiplier;
		public int			cityDropoff;
		public int			cityDepth;
		public float		citySpread;
		public float		cityIntensity;
		public float		maxCityIntensity;
		public float		cityFalloff;

		public float		normalScale;

		public PlanetNoise	cloudNoise;
		public int			cloudWorleyNoiseOctaves;
		public float		cloudWorleyNoiseFrequency;
		public float		cloudWorleyNoiseAmplitude;
		public float		cloudShadowRange;
		public float		cloudShadowStrength;
		public Color		cloudColour0;
		public Color		cloudColour1;
		public float		cloudSpin;
		public float		cloudNormalScale;

		public int			size;
		public bool			combineHeightWithCity;

		public void OnEnable()
		{
			if( surfaceNoise == null )
			{
				surfaceNoise						= new PlanetNoise();
				landNoise							= new PlanetNoise();
				landColourNoise01					= new PlanetNoise();
				landColourNoise23					= new PlanetNoise();
				cloudNoise							= new PlanetNoise();


				seedSurface							= Random.Range( 0, int.MaxValue );
				seedLand							= Random.Range( 0, int.MaxValue );
				seedLandColour01					= Random.Range( 0, int.MaxValue );
				seedLandColour23					= Random.Range( 0, int.MaxValue );
				seedCity							= Random.Range( 0, int.MaxValue );
				seedCloud							= Random.Range( 0, int.MaxValue );


				surfaceNoise.scale					= 0.9f;
				surfaceNoise.octaves				= 10;
				surfaceNoise.falloff				= 4.94f;
				surfaceNoise.intensity				= 2.75f;
				surfaceNoise.ridginess				= 0.46f;
				surfaceNoise.smearScale				= 0.6f;
				surfaceNoise.smearOctaves			= 1;
				surfaceNoise.smearFalloff			= 7.9f;
				surfaceNoise.smearIntensity			= 0.8f;


				landNoise.scale						= 0.68f;
				landNoise.octaves					= 1;
				landNoise.falloff					= 3.74f;
				landNoise.intensity					= 3.09f;
				landNoise.ridginess					= 0f;
				landNoise.smearScale				= 2f;
				landNoise.smearOctaves				= 1;
				landNoise.smearFalloff				= 1.04f;
				landNoise.smearIntensity			= 1.61f;

				landColour0							= new Color( 152f / 255f, 107f / 255f, 71f / 255f );
				landColour1							= new Color( 69f / 255f, 43f / 255f, 22f / 255f );
				landColour2							= new Color( 0f / 255f, 20f / 255f, 0f / 255f );
				landColour3							= new Color( 11f / 255f, 64f / 255f, 11f / 255f );

				landColourNoise01.scale				= 0.49f;
				landColourNoise01.octaves			= 9;
				landColourNoise01.falloff			= 9.13f;
				landColourNoise01.intensity			= 16.16f;
				landColourNoise01.ridginess			= 0.234f;
				landColourNoise01.smearScale		= 19.76f;
				landColourNoise01.smearOctaves		= 1;
				landColourNoise01.smearFalloff		= 9.53f;
				landColourNoise01.smearIntensity	= 15.73f;

				landColourNoise23.scale				= 0.49f;
				landColourNoise23.octaves			= 9;
				landColourNoise23.falloff			= 9.13f;
				landColourNoise23.intensity			= 16.16f;
				landColourNoise23.ridginess			= 0.234f;
				landColourNoise23.smearScale		= 19.76f;
				landColourNoise23.smearOctaves		= 1;
				landColourNoise23.smearFalloff		= 9.53f;
				landColourNoise23.smearIntensity	= 15.73f;


				waterColour0						= new Color( 3f / 255f, 3f / 255f, 52f / 255f );
				waterColour1						= new Color( 0f / 255f, 0f / 255f, 88f / 255f );
				waterLevel							= 0.5f;
				waterSpecular						= 1f;
				waterFalloff						= 3.1f;


				iceColour							= new Color( 1f, 1f, 234f / 255f, 1f );
				iceReach							= 0.308f;
				iceHeight							= 0.986f;


				shadowRange							= 20f;
				shadowStrength						= 0.4f;


				cityReach							= 0.8f;
				cityHeight							= 0.8f;
				cityColour							= new Color( 37f / 255f, 27f / 255f, 17f / 255f );
				cityCount							= 128;
				cityMultiplier						= 6;
				cityDropoff							= 4;
				cityDepth							= 4;
				citySpread							= 0.45f;
				cityIntensity						= 64f;
				maxCityIntensity					= 7.5f;
				cityFalloff							= 0.5f;


				normalScale							= 0.1f;


				cloudNoise.scale					= 0.4f;
				cloudNoise.octaves					= 10;
				cloudNoise.falloff					= 4.1f;
				cloudNoise.intensity				= 6.1f;
				cloudNoise.ridginess				= 0f;
				cloudNoise.smearScale				= 16.38f;
				cloudNoise.smearOctaves				= 3;
				cloudNoise.smearFalloff				= 4.2f;
				cloudNoise.smearIntensity			= 5.15f;

				cloudWorleyNoiseOctaves				= 1;
				cloudWorleyNoiseFrequency			= 6f;
				cloudWorleyNoiseAmplitude			= 0.5f;

				cloudShadowRange					= 10f;
				cloudShadowStrength					= 0.1f;

				cloudColour0						= new Color( 1f, 1f, 1f, 1f );
				cloudColour1						= new Color( 0f, 0f, 0f, 0f );

				cloudSpin							= 1f;
				cloudNormalScale					= 0.01f;
				

				size								= 0;
				combineHeightWithCity				= true;
			}
		}

		public void randomise()
		{
			randomiseSeeds();


			randomiseNoise( surfaceNoise );


			randomiseNoise( landNoise );
			landColour0					= randomColour();
			landColour1					= randomColour();
			landColour2					= randomColour();
			landColour3					= randomColour();
			randomiseNoise( landColourNoise01 );
			randomiseNoise( landColourNoise23 );


			waterColour0				= randomColour();
			waterColour1				= randomColour();
			waterLevel					= Random.Range( 0f,		1f );
			waterSpecular				= Random.Range( 0f,		1f );
			waterFalloff				= Random.Range( 0.1f,	20 );


			iceColour					= randomColour();
			iceReach					= Random.Range( 0f,		1f );
			iceHeight					= Random.Range( 0f,		1f );


			shadowRange					= Random.Range( 0f,		100f );
			shadowStrength				= Random.Range( 0f,		1f );


			cityReach					= Random.Range( 0f,		1f );
			cityHeight					= Random.Range( 0f,		1f );
			cityColour					= randomColour();
			cityCount					= Random.Range( 0,		32 );
			cityMultiplier				= Random.Range( 1,		6 );
			cityDropoff					= Random.Range( 1,		10 );
			cityDepth					= Random.Range( 1,		5 );
			citySpread					= Random.Range( 0.25f,	4f );
			cityIntensity				= Random.Range( 8f,		128f );
			maxCityIntensity			= Random.Range( 4f,		64f );
			cityFalloff					= Random.Range( 0.25f,	2f );


			randomiseNoise( cloudNoise );
			cloudWorleyNoiseOctaves		= Random.Range( 0,		10 );
			cloudWorleyNoiseFrequency	= Random.Range( 1f,		100f );
			cloudWorleyNoiseAmplitude	= Random.Range( 1f,		10f );
			

			cloudShadowRange			= Random.Range( 0f,		100f );
			cloudShadowStrength			= Random.Range( 0f,		1f );


			cloudColour0				= randomColour();
			cloudColour1				= randomColour();
			cloudSpin					= Random.Range( 1f,		10f );
		}

		public void randomiseSeeds()
		{
			seedSurface	= Random.Range( 0, int.MaxValue );
			seedCity	= Random.Range( 0, int.MaxValue );
			seedCloud	= Random.Range( 0, int.MaxValue );
		}

		public void randomiseNoise( PlanetNoise noise )
		{
			noise.scale				= Random.Range( 0.1f,	20f );
			noise.octaves			= Random.Range( 0,		10 );
			noise.falloff			= Random.Range( 0f,		20f );
			noise.intensity			= Random.Range( 0f,		20f );
			noise.ridginess			= Random.Range( 0f,		1f );
			noise.smearScale		= Random.Range( 0.1f,	20f );
			noise.smearOctaves		= Random.Range( 0,		10 );
			noise.smearFalloff		= Random.Range( 0f,		20f );
			noise.smearIntensity	= Random.Range( 0f,		20f );
		}
		
		public Color randomColour() { return new Color( Random.Range( 0f, 1f ), Random.Range( 0f, 1f ), Random.Range( 0f, 1f ) ); }
	}


	[ System.Serializable ]
	public class PlanetNoise
	{
		public float	scale;
		public int		octaves;
		public float	falloff;
		public float	intensity;
		public float	ridginess;
		public float	smearScale;
		public int		smearOctaves;
		public float	smearFalloff;
		public float	smearIntensity;
	}
}