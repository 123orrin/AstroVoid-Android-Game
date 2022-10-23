#define USE_GPU

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetNoiseGenerator
	{
		public const string	computeShaderPath		= "Assets/HeyBlairGames/PlanetTextureGenerator/Editor/PlanetNoiseGenerator.compute";

		//this controls how many threads are run per shader dispatch,
		//too many and the graphics driver will crash but this is
		//dependent on your gpu, if your graphics driver is crashing
		//while generating textures, lower this number
		public const int	maxThreadsPerDispatch	= 1024 * 512;

		//remember to update the numthreads count for each
		//kernel (in shader) to match the numbers here
		public const int	diffuseThreadsPerGroup	= 128;
		public const int	cloudThreadsPerGroup	= 128;

		public const int	diffusePixelsPerThread	= 3;		//rgbaxyzhiiii
		public const int	cloudPixelsPerThread	= 2;		//rgbaxyzh


		public delegate void WorkAction( int jobId, int jobCount );

		public byte[]	diffuseData;
		public float[]	cityData;
		public byte[]	cloudData;

		public void createNoise( int seedSurface, int seedLand, int seedLandColour01, int seedLandColour23, int seedCity, int seedCloud )
		{
			perlinNoiseSurface		= new PlanetNoisePerlin( seedSurface );
			perlinNoiseLand			= new PlanetNoisePerlin( seedLand );
			perlinNoiseLandColour01	= new PlanetNoisePerlin( seedLandColour01 );
			perlinNoiseLandColour23	= new PlanetNoisePerlin( seedLandColour23 );

			_cityRnd				= new System.Random( seedCity );

			perlinNoiseCloud		= new PlanetNoisePerlin( seedCloud );
			
			worleyNoiseCloud		= new PlanetNoiseWorley( seedCloud );

			isDoingCityWork			= false;

#if USE_GPU
			if( noiseComputeShader == null )
			{
				useComputerShader = false;

				if( SystemInfo.supportsComputeShaders )
				{
					useComputerShader	= true;

					noiseComputeShader	= ( ComputeShader ) AssetDatabase.LoadAssetAtPath( computeShaderPath, typeof( ComputeShader ) );

					if( noiseComputeShader )
					{
						_diffuseKernel	= noiseComputeShader.FindKernel( "diffuseNoiseKernel" );
						_cloudKernel	= noiseComputeShader.FindKernel( "cloudNoiseKernel" );
					}
					else
					{
						Debug.LogError( "couldn't load " + computeShaderPath );
						useComputerShader = false;
					}
				}
			}
#endif
		}

		public void generateDiffuseNoise( int width, int height,
											PlanetNoise surfaceParams,
											PlanetNoise landParams,
											Color[] landColour,
											PlanetNoise[] landColourNoise,
											float cityReach,
											float cityHeight,
											Color waterColour0,
											Color waterColour1,
											float waterLevel,
											float waterSpecular,
											float waterFalloff,
											Color iceColour,
											float iceReach,
											float iceHeight,
											float shadowRange,
											float shadowStrength,
											float normalScale )
		{
			_width				= width;
			_height				= height;
			_surfaceParams		= surfaceParams;
			_landParams			= landParams;
			_landColour			= landColour;
			_landColourNoise	= landColourNoise;
			_cityReach			= cityReach * 0.5f;
			_cityHeight			= cityHeight;
			_waterColour0		= waterColour0;
			_waterColour1		= waterColour1;
			_waterLevel			= waterLevel;
			_waterSpecular		= waterSpecular;
			_waterFalloff		= waterFalloff;
			_iceColour			= iceColour;
			_iceReach			= iceReach * 0.5f;
			_iceHeight			= iceHeight;
			_shadowRange		= shadowRange;
			_shadowStrength		= shadowStrength;
			_normalScale		= normalScale;
			
			//9 bytes per pixel - all colour data followed by all normal data followed by all illumination data
			//colour - r, g, b, a
			//normal - x, y, z, height
			//illumination - i
			int bytesPerPixel	= 9;
			int pixelCount		= width * height;

			if( diffuseData == null || diffuseData.Length != pixelCount * bytesPerPixel )
				diffuseData = new byte[ pixelCount * bytesPerPixel ];
			
			if( useComputerShader )
			{
				noiseComputeShader.SetFloat( "width",	width );
				noiseComputeShader.SetFloat( "height",	height );

				//diffuse perlin noise data
				int gradBytes		= sizeof( float );
				int gradStride		= PlanetNoisePerlin.gradStride;
				float[] gradArray	= new float[ gradStride * 4 ];

				System.Buffer.BlockCopy( perlinNoiseSurface.getGradArray(),			0, gradArray, 0,							gradStride * gradBytes );
				System.Buffer.BlockCopy( perlinNoiseLand.getGradArray(),			0, gradArray, gradStride * gradBytes,		gradStride * gradBytes );
				System.Buffer.BlockCopy( perlinNoiseLandColour01.getGradArray(),	0, gradArray, gradStride * gradBytes * 2,	gradStride * gradBytes );
				System.Buffer.BlockCopy( perlinNoiseLandColour23.getGradArray(),	0, gradArray, gradStride * gradBytes * 3,	gradStride * gradBytes );

				perlinGradBuffer = new ComputeBuffer( gradArray.Length, gradBytes );
				perlinGradBuffer.SetData( gradArray );
				noiseComputeShader.SetBuffer( _diffuseKernel, "perlinGrad", perlinGradBuffer );

				int permBytes		= sizeof( int );
				int permStride		= PlanetNoisePerlin.permStride;
				uint[] permArray	= new uint[ permStride * 4 ];

				System.Buffer.BlockCopy( perlinNoiseSurface.getPermArray(),			0, permArray, 0,							permStride * permBytes );
				System.Buffer.BlockCopy( perlinNoiseLand.getPermArray(),			0, permArray, permStride * permBytes,		permStride * permBytes );
				System.Buffer.BlockCopy( perlinNoiseLandColour01.getPermArray(),	0, permArray, permStride * permBytes * 2,	permStride * permBytes );
				System.Buffer.BlockCopy( perlinNoiseLandColour23.getPermArray(),	0, permArray, permStride * permBytes * 3,	permStride * permBytes );

				perlinPermBuffer = new ComputeBuffer( permArray.Length, permBytes );
				perlinPermBuffer.SetData( permArray );
				noiseComputeShader.SetBuffer( _diffuseKernel, "perlinPerm", perlinPermBuffer );

				//surface noise data
				noiseComputeShader.SetFloat( "surfaceScale",			surfaceParams.scale );
				noiseComputeShader.SetInt( "surfaceOctaves",			surfaceParams.octaves );
				noiseComputeShader.SetFloat( "surfaceFalloff",			surfaceParams.falloff );
				noiseComputeShader.SetFloat( "surfaceIntensity",		surfaceParams.intensity );
				noiseComputeShader.SetFloat( "surfaceRidginess",		surfaceParams.ridginess );
				noiseComputeShader.SetFloat( "surfaceSmearScale",		surfaceParams.smearScale );
				noiseComputeShader.SetInt( "surfaceSmearOctaves",		surfaceParams.smearOctaves );
				noiseComputeShader.SetFloat( "surfaceSmearFalloff",		surfaceParams.smearFalloff );
				noiseComputeShader.SetFloat( "surfaceSmearIntensity",	surfaceParams.smearIntensity );

				//land noise data
				noiseComputeShader.SetFloat( "landScale",			landParams.scale );
				noiseComputeShader.SetInt( "landOctaves",			landParams.octaves );
				noiseComputeShader.SetFloat( "landFalloff",			landParams.falloff );
				noiseComputeShader.SetFloat( "landIntensity",		landParams.intensity );
				noiseComputeShader.SetFloat( "landRidginess",		landParams.ridginess );
				noiseComputeShader.SetFloat( "landSmearScale",		landParams.smearScale );
				noiseComputeShader.SetInt( "landSmearOctaves",		landParams.smearOctaves );
				noiseComputeShader.SetFloat( "landSmearFalloff",	landParams.smearFalloff );
				noiseComputeShader.SetFloat( "landSmearIntensity",	landParams.smearIntensity );

				//land colours
				for( int i = 0; i < landColour.Length; ++i )
					noiseComputeShader.SetFloats( "landColour" + i, new float[] { landColour[ i ].r, landColour[ i ].g, landColour[ i ].b, landColour[ i ].a } );

				for( int i = 0; i < landColourNoise.Length; ++i )
				{
					string id = ( i * 2 ).ToString() + ( i * 2 + 1 ).ToString();

					//land colour noise data
					noiseComputeShader.SetFloat( "landColour" + id + "Scale",			landColourNoise[ i ].scale );
					noiseComputeShader.SetInt( "landColour" + id + "Octaves",			landColourNoise[ i ].octaves );
					noiseComputeShader.SetFloat( "landColour" + id + "Falloff",			landColourNoise[ i ].falloff );
					noiseComputeShader.SetFloat( "landColour" + id + "Intensity",		landColourNoise[ i ].intensity );
					noiseComputeShader.SetFloat( "landColour" + id + "Ridginess",		landColourNoise[ i ].ridginess );
					noiseComputeShader.SetFloat( "landColour" + id + "SmearScale",		landColourNoise[ i ].smearScale );
					noiseComputeShader.SetInt( "landColour" + id + "SmearOctaves",		landColourNoise[ i ].smearOctaves );
					noiseComputeShader.SetFloat( "landColour" + id + "SmearFalloff",	landColourNoise[ i ].smearFalloff );
					noiseComputeShader.SetFloat( "landColour" + id + "SmearIntensity",	landColourNoise[ i ].smearIntensity );
				}

				//diffuse params
				noiseComputeShader.SetFloat( "cityReach",		_cityReach );
				noiseComputeShader.SetFloat( "cityHeight",		cityHeight );
				noiseComputeShader.SetFloats( "waterColour0",	new float[] { waterColour0.r, waterColour0.g, waterColour0.b, waterColour0.a } );
				noiseComputeShader.SetFloats( "waterColour1",	new float[] { waterColour1.r, waterColour1.g, waterColour1.b, waterColour1.a } );
				noiseComputeShader.SetFloat( "waterLevel",		waterLevel );
				noiseComputeShader.SetFloat( "waterSpecular",	waterSpecular );
				noiseComputeShader.SetFloat( "waterFalloff",	waterFalloff );
				noiseComputeShader.SetFloats( "iceColour",		new float[] { iceColour.r, iceColour.g, iceColour.b, iceColour.a } );
				noiseComputeShader.SetFloat( "iceReach",		_iceReach );
				noiseComputeShader.SetFloat( "iceHeight",		iceHeight );
				noiseComputeShader.SetFloat( "shadowRange",		shadowRange );
				noiseComputeShader.SetFloat( "shadowStrength",	shadowStrength );
				noiseComputeShader.SetFloat( "normalScale",		normalScale );
				
				int pixelsPerDispatch = pixelCount;

				if( pixelsPerDispatch > maxThreadsPerDispatch )
				{
					pixelsPerDispatch = maxThreadsPerDispatch;
					pixelsPerDispatch -= maxThreadsPerDispatch % diffuseThreadsPerGroup;
				}

				noiseComputeShader.SetInt( "threadOffset", 0 );

				_jobCount			= Mathf.CeilToInt( pixelCount / ( float ) pixelsPerDispatch );
				_jobStartedCount	= 1;	//first dispatch is below
				_jobCompletedCount	= 0;

				computeResultBuffer = new ComputeBuffer( pixelsPerDispatch * diffusePixelsPerThread, sizeof( int ) );
				noiseComputeShader.SetBuffer( _diffuseKernel, "resultData", computeResultBuffer );

				noiseComputeShader.Dispatch( _diffuseKernel, pixelsPerDispatch / diffuseThreadsPerGroup, 1, 1 );

				computeResultData = new int[ pixelsPerDispatch * diffusePixelsPerThread ];
			}
			else
			{
				int workerCount = SystemInfo.processorCount - 1;				//-1 to account for main thread

				if( workerCount > 0 )
					_worker = new Thread[ workerCount ];

				//a bit inefficient to have more jobs than workers but with multiple threads
				//per core nowadays it makes sense, also gives an easy way to measure progress
				_jobCount			= Mathf.Max( 32, ( workerCount + 1 ) * ( workerCount + 1 ) );
				_jobCount			= Mathf.Min( width, _jobCount );
				_jobStartedCount	= 0;
				_jobCompletedCount	= 0;
				_jobList			= new int[ _jobCount ];

				for( int i = 0; i < _jobCount; ++i )
					_jobList[ i ] = i;

				for( int i = 0; i < workerCount; ++i )
				{
					_worker[ i ] = new Thread( () => { while( doDiffuseWork() ); } );
					_worker[ i ].Start();
				}
			}
		}

		public bool doDiffuseWork()
		{
			if( useComputerShader )
			{
				int pixelCount = _width * _height;

				computeResultBuffer.GetData( computeResultData );

				int pixelsPerDispatch	= computeResultData.Length / diffusePixelsPerThread;
				int offset				= _jobCompletedCount * pixelsPerDispatch;

				++_jobCompletedCount;

				if( _jobCompletedCount < _jobCount )
				{
					noiseComputeShader.SetInt( "threadOffset", _jobCompletedCount * ( computeResultData.Length / diffusePixelsPerThread ) );
					noiseComputeShader.Dispatch( _diffuseKernel, pixelsPerDispatch / diffuseThreadsPerGroup, 1, 1 );
				}

				//byte layout - rgbaxyzhiiii
				for( int i = 0; i < computeResultData.Length; ++i )
				{
					int colour = computeResultData[ i ];

					int diffuseIndex = ( offset + ( i / diffusePixelsPerThread ) ) * 4;

					if( _jobStartedCount == _jobCount && diffuseIndex >= pixelCount * 4 )
						break;

					int r = i % 3;
					if( r == 2 )
					{
						diffuseIndex /= 4;
						diffuseIndex += pixelCount * 8;

						diffuseData[ diffuseIndex ]		= ( byte ) ( ( colour & 0xff000000 ) >> 24 );
					}
					else
					{
						if( r == 1 )
							diffuseIndex += pixelCount * 4;

						diffuseData[ diffuseIndex ]		= ( byte ) ( ( colour & 0xff000000 ) >> 24 );
						diffuseData[ diffuseIndex + 1 ]	= ( byte ) ( ( colour & 0x00ff0000 ) >> 16 );
						diffuseData[ diffuseIndex + 2 ]	= ( byte ) ( ( colour & 0x0000ff00 ) >> 8 );
						diffuseData[ diffuseIndex + 3 ]	= ( byte ) ( colour & 0x000000ff );
					}
				}

				if( _jobCompletedCount < _jobCount )
					return true;
				
				return false;
			}
			else
			{
				bool result = doWork( diffuseNoiseWork );
				return result;
			}
		}

		//this is faster on the cpu than the gpu so don't bother with the compute shader here
		//cityCount - no of cities to start with, cityMultiplier - how much cityCount increases by with each layer, cityDepth - how many layers
		//citySpread - radius of city, cityIntensity - max value of city, cityFalloff - how quickly cityIntensity drops
		public void generateCityNoise( int width, int height,
										int cityCount,
										int cityMultiplier,
										int cityDropoff,
										int cityDepth,
										float citySpread,
										float cityIntensity,
										float maxIntensity,
										float cityFalloff )
		{
			_width					= width;
			_height					= height;
			_cityMaxIntensity		= maxIntensity;
			_cityFalloff			= cityFalloff;
			_diffuseStartIndexC		= width * height * 8;

			int pixelCount			= width * height;
			cityData				= new float[ pixelCount ];

			citySpread				*= height * 0.01f;
			float startCitySpread	= citySpread;
			float invCityDropoff	= 1f / ( float ) cityDropoff;

			if( cityCount <= 0 )
				return;

			//set up array for city positions
			int count = cityCount;

			for( int i = 0; i < cityDepth; ++i )
				count *= cityMultiplier;

			Vector2[,]	cityPos			= new Vector2[ 2, count ];
			Vector2[]	startCityPos	= new Vector2[ cityCount ];

			int cityIndexOld			= 0;
			int cityIndexNew			= 1;

			cityPos[ cityIndexOld, 0 ]	= new Vector2( Mathf.RoundToInt( width / 2f ), Mathf.RoundToInt( height / 2f ) );

			_citySplatList				= new List< CitySplatData >();

			//set up offsets for positions
			int xOffset	= width;
			int yOffset	= height;

			float angle	= 360f / ( float ) cityMultiplier;

			for( int d = 0; d < cityDepth; ++d )
			{
				for( int i = 0; i < cityCount; ++i )
				{
					int index	= Mathf.FloorToInt( i / ( float ) cityMultiplier );
					Vector2 pos	= cityPos[ cityIndexOld, index ];

					int x		= ( _cityRnd.Next( xOffset ) - Mathf.RoundToInt( xOffset * 0.5f ) ) + Mathf.RoundToInt( pos.x );
					int y		= ( _cityRnd.Next( yOffset ) - Mathf.RoundToInt( yOffset * 0.5f ) ) + Mathf.RoundToInt( pos.y );

					if( d > 0 )
					{
						float a		= Mathf.Deg2Rad * ( angle * i + ( _cityRnd.Next( 80 ) - 40 ) );
						float diff	= ( 0.5f - getSphericalDistortion( y ) ) * 2f;

						if( diff <= 0f )
							diff = 1f;

						float xDiff	= Mathf.Min( _cityRnd.Next( Mathf.RoundToInt( xOffset ) ) / diff, width );
						float yDiff	= _cityRnd.Next( Mathf.RoundToInt( yOffset ) );
						x			= Mathf.RoundToInt( Mathf.Cos( a ) * ( xOffset * 0.25f + xDiff ) + pos.x );
						y			= Mathf.RoundToInt( Mathf.Sin( a ) * ( yOffset * 0.25f + yDiff ) + pos.y );
					}

					while( x < 0 || x >= _width )
						x = ( x + _width ) % _width;

					while( y < 0 || y >= _height )
						y = ( y + _height ) % _height;

					float spread = Mathf.Min( citySpread + _cityRnd.Next( cityDropoff ), startCitySpread );
					
					CitySplatData citySplat	= new CitySplatData();
					citySplat.x				= x;
					citySplat.y				= y;
					citySplat.spread		= spread;
					citySplat.intensity		= cityIntensity;
					_citySplatList.Add( citySplat );

					cityPos[ cityIndexNew, i ] = new Vector2( x, y );

					if( d == 0 )
						startCityPos[ i ] = new Vector2( x, y );
				}

				if( d < cityDepth - 1 )
				{
					cityCount		*= cityMultiplier;
					citySpread		*= invCityDropoff * 2f;
					cityIntensity	= cityIntensity * invCityDropoff * 2f;

					cityIndexOld	= ++cityIndexOld & 1;
					cityIndexNew	= ++cityIndexNew & 1;

					xOffset			= Mathf.RoundToInt( citySpread ) * cityMultiplier * 2;
					yOffset			= xOffset;
				}
			}

			cityCount = startCityPos.Length * cityMultiplier;
			float startCityPosLength = startCityPos.Length / ( float ) cityCount;

			for( int i = 0; i < cityCount; ++i )
			{
				int index		= Mathf.FloorToInt( i * startCityPosLength );
				Vector2 pos		= startCityPos[ index ];

				float a			= Mathf.Deg2Rad * ( angle * i + ( _cityRnd.Next( 80 ) - 40 ) );
				int x			= Mathf.RoundToInt( Mathf.Cos( a ) * ( startCitySpread + _cityRnd.Next( Mathf.RoundToInt( startCitySpread * 0.25f ) ) ) + pos.x );
				int y			= Mathf.RoundToInt( Mathf.Sin( a ) * ( startCitySpread + _cityRnd.Next( Mathf.RoundToInt( startCitySpread * 0.25f ) ) ) + pos.y );

				while( x < 0 || x >= _width )
					x = ( x + _width ) % _width;

				while( y < 0 || y >= _height )
					y = ( y + _height ) % _height;

				float spread	= Mathf.Min( citySpread + _cityRnd.Next( Mathf.RoundToInt( cityDropoff * 2 ) ), startCitySpread );

				CitySplatData citySplat	= new CitySplatData();
				citySplat.x				= x;
				citySplat.y				= y;
				citySplat.spread		= spread;
				citySplat.intensity		= cityIntensity;
				_citySplatList.Add( citySplat );
			}

			int workerCount = SystemInfo.processorCount - 1;				//-1 to account for main thread

			if( workerCount > 0 )
				_worker = new Thread[ workerCount ];

			//a bit inefficient to have more jobs than workers but with multiple threads
			//per core nowadays it makes sense, also gives an easy way to measure progress
			_tileCountX			= ( workerCount + 1 ) * 2;										//this gives us the no. of horizontal tiles
			_tileCountX			= Mathf.Min( width / 16, _tileCountX );							//this means the tiles can't be smaller than 16 pixels wide
			_tileCountY			= _tileCountX / 2;												//this gives us square tiles
			_jobCount			= _tileCountX * _tileCountY;
			_jobStartedCount	= 0;
			_jobCompletedCount	= 0;
			_jobList			= new int[ _jobCount ];

			for( int i = 0; i < _jobCount; ++i )
				_jobList[ i ] = i;

			for( int i = 0; i < workerCount; ++i )
			{
				_worker[ i ] = new Thread( () => { while( doCityWork() ); } );
				_worker[ i ].Start();
			}

			isDoingCityWork = true;
		}

		public bool doCityWork()
		{
			bool result = doWork( cityNoiseWork );
			return result;
		}

		public void generateCloudNoise( int width, int height,
										PlanetNoise cloudParams,
										int cloudWorleyNoiseOctaves,
										float cloudWorleyNoiseFrequency,
										float cloudWorleyNoiseAmplitude,
										float cloudShadowRange,
										float cloudShadowStrength,
										Color cloudColour0,
										Color cloudColour1,
										float cloudSpin,
										float cloudNormalScale,
										bool preMultiplyAlpha )
		{
			_width						= width;
			_height						= height;
			_cloudWorleyNoiseOctaves	= cloudWorleyNoiseOctaves;
			_cloudWorleyNoiseFrequency	= cloudWorleyNoiseFrequency;
			_cloudWorleyNoiseAmplitude	= cloudWorleyNoiseAmplitude;
			_cloudShadowRange			= cloudShadowRange;
			_cloudShadowStrength		= cloudShadowStrength;
			_cloudParams				= cloudParams;
			_cloudColour0				= cloudColour0;
			_cloudColour1				= cloudColour1;
			_cloudSpin					= cloudSpin;
			_cloudNormalScale			= cloudNormalScale;
			_preMultiplyAlpha			= preMultiplyAlpha;

			//8 bytes per pixel - all colour data followed by all normal data
			//colour - r, g, b, a
			//normal - x, y, z, height
			int bytesPerPixel	= 8;
			int pixelCount		= width * height;

			if( cloudData == null || cloudData.Length != pixelCount * bytesPerPixel )
				cloudData = new byte[ pixelCount * bytesPerPixel ];

			if( useComputerShader )
			{
				noiseComputeShader.SetFloat( "width",	width );
				noiseComputeShader.SetFloat( "height",	height );

				//perlin noise data
				float[] gradArray	= perlinNoiseCloud.getGradArray();
				perlinGradBuffer	= new ComputeBuffer( gradArray.Length, sizeof( float ) );
				perlinGradBuffer.SetData( gradArray );
				noiseComputeShader.SetBuffer( _cloudKernel, "perlinGrad", perlinGradBuffer );

				uint[] permArray	= perlinNoiseCloud.getPermArray();
				perlinPermBuffer	= new ComputeBuffer( permArray.Length, sizeof( int ) );
				perlinPermBuffer.SetData( permArray );
				noiseComputeShader.SetBuffer( _cloudKernel, "perlinPerm", perlinPermBuffer );

				noiseComputeShader.SetInt( "cloudSeed", worleyNoiseCloud.seed );

				//cloud noise data
				noiseComputeShader.SetFloat( "cloudScale",			cloudParams.scale );
				noiseComputeShader.SetInt( "cloudOctaves",			cloudParams.octaves );
				noiseComputeShader.SetFloat( "cloudFalloff",		cloudParams.falloff );
				noiseComputeShader.SetFloat( "cloudIntensity",		cloudParams.intensity );
				noiseComputeShader.SetFloat( "cloudRidginess",		cloudParams.ridginess );
				noiseComputeShader.SetFloat( "cloudSmearScale",		cloudParams.smearScale );
				noiseComputeShader.SetInt( "cloudSmearOctaves",		cloudParams.smearOctaves );
				noiseComputeShader.SetFloat( "cloudSmearFalloff",	cloudParams.smearFalloff );
				noiseComputeShader.SetFloat( "cloudSmearIntensity",	cloudParams.smearIntensity );

				//cloud worley noise data
				noiseComputeShader.SetInt( "cloudWorleyNoiseOctaves",		_cloudWorleyNoiseOctaves );
				noiseComputeShader.SetFloat( "cloudWorleyNoiseFrequency",	_cloudWorleyNoiseFrequency );
				noiseComputeShader.SetFloat( "cloudWorleyNoiseAmplitude",	_cloudWorleyNoiseAmplitude );

				noiseComputeShader.SetFloat( "cloudShadowRange",	_cloudShadowRange );
				noiseComputeShader.SetFloat( "cloudShadowStrength",	_cloudShadowStrength );

				//cloud params
				noiseComputeShader.SetFloats( "cloudColour0",		new float[] { cloudColour0.r, cloudColour0.g, cloudColour0.b, cloudColour0.a } );
				noiseComputeShader.SetFloats( "cloudColour1",		new float[] { cloudColour1.r, cloudColour1.g, cloudColour1.b, cloudColour1.a } );
				noiseComputeShader.SetFloat( "cloudSpin",			cloudSpin );
				noiseComputeShader.SetFloat( "cloudNormalScale",	cloudNormalScale );
				noiseComputeShader.SetInt( "preMultiplyAlpha",		preMultiplyAlpha ? 1 : 0 );
				
				int pixelsPerDispatch = pixelCount;

				if( pixelsPerDispatch > maxThreadsPerDispatch )
				{
					pixelsPerDispatch = maxThreadsPerDispatch;
					pixelsPerDispatch -= maxThreadsPerDispatch % cloudThreadsPerGroup;
				}

				noiseComputeShader.SetInt( "threadOffset", 0 );

				_jobCount			= Mathf.CeilToInt( pixelCount / ( float ) pixelsPerDispatch );
				_jobStartedCount	= 1;	//first dispatch is below
				_jobCompletedCount	= 0;

				computeResultBuffer = new ComputeBuffer( pixelsPerDispatch * cloudPixelsPerThread, sizeof( int ) );
				noiseComputeShader.SetBuffer( _cloudKernel, "resultData", computeResultBuffer );

				noiseComputeShader.Dispatch( _cloudKernel, pixelsPerDispatch / cloudThreadsPerGroup, 1, 1 );

				computeResultData = new int[ pixelsPerDispatch * cloudPixelsPerThread ];
			}
			else
			{
				int workerCount = SystemInfo.processorCount - 1;				//-1 to account for main thread

				if( workerCount > 0 )
					_worker = new Thread[ workerCount ];

				//a bit inefficient to have more jobs than workers but with multiple threads
				//per core nowadays it makes sense, also gives an easy way to measure progress
				_jobCount			= Mathf.Max( 32, ( workerCount + 1 ) * ( workerCount + 1 ) );
				_jobCount			= Mathf.Min( width, _jobCount );
				_jobStartedCount	= 0;
				_jobCompletedCount	= 0;
				_jobList			= new int[ _jobCount ];

				for( int i = 0; i < _jobCount; ++i )
					_jobList[ i ] = i;

				for( int i = 0; i < workerCount; ++i )
				{
					_worker[ i ] = new Thread( () => { while( doCloudWork() ); } );
					_worker[ i ].Start();
				}
			}
		}

		public bool doCloudWork()
		{
			if( useComputerShader )
			{
				int pixelCount = _width * _height;

				computeResultBuffer.GetData( computeResultData );

				int pixelsPerDispatch	= computeResultData.Length / cloudPixelsPerThread;
				int offset				= _jobCompletedCount * pixelsPerDispatch;

				++_jobCompletedCount;

				if( _jobCompletedCount < _jobCount )
				{
					noiseComputeShader.SetInt( "threadOffset", _jobCompletedCount * ( computeResultData.Length / cloudPixelsPerThread ) );
					noiseComputeShader.Dispatch( _cloudKernel, pixelsPerDispatch / cloudThreadsPerGroup, 1, 1 );
				}

				//byte layout - rgbaxyzh
				for( int i = 0; i < computeResultData.Length; ++i )
				{
					int colour = computeResultData[ i ];

					int cloudIndex = ( offset + ( i / cloudPixelsPerThread ) ) * 4;

					if( _jobStartedCount == _jobCount && cloudIndex >= pixelCount * 4 )
						break;

					if( ( i & 0x1 ) == 1 )
						cloudIndex += pixelCount * 4;

					cloudData[ cloudIndex ]		= ( byte ) ( ( colour & 0xff000000 ) >> 24 );
					cloudData[ cloudIndex + 1 ]	= ( byte ) ( ( colour & 0x00ff0000 ) >> 16 );
					cloudData[ cloudIndex + 2 ]	= ( byte ) ( ( colour & 0x0000ff00 ) >> 8 );
					cloudData[ cloudIndex + 3 ]	= ( byte ) ( colour & 0x000000ff );
				}

				if( _jobCompletedCount < _jobCount )
					return true;
				
				return false;
			}
			else
			{
				bool result = doWork( cloudNoiseWork );
				return result;
			}
		}

		public void waitForGenerator()
		{
			if( useComputerShader && !isDoingCityWork )
			{
				computeResultData = null;

				if( perlinGradBuffer != null )
				{
					perlinGradBuffer.Release();
					perlinGradBuffer = null;
					perlinPermBuffer.Release();
					perlinPermBuffer = null;
				}

				if( computeResultBuffer != null )
				{
					computeResultBuffer.Release();
					computeResultBuffer = null;
				}
			}
			else
			{
				while( _jobCompletedCount < _jobCount )
					Thread.Sleep( 100 );

				//cleanup worker threads
				_worker = null;

				isDoingCityWork = false;
			}

			//cleanup job data
			_jobCount			= 0;
			_jobStartedCount	= 0;
			_jobCompletedCount	= 0;
			_jobList			= null;
		}

		public float getProgress()
		{
			float result = _jobCompletedCount / ( float ) _jobCount;
			return result;
		}


		private PlanetNoisePerlin		perlinNoiseSurface;
		private PlanetNoisePerlin		perlinNoiseLand;
		private PlanetNoisePerlin		perlinNoiseLandColour01;
		private PlanetNoisePerlin		perlinNoiseLandColour23;
		private PlanetNoisePerlin		perlinNoiseCloud;
		private PlanetNoiseWorley		worleyNoiseCloud;

		private bool					useComputerShader			= false;
		private ComputeShader			noiseComputeShader			= null;
		private ComputeBuffer			perlinGradBuffer;
		private ComputeBuffer			perlinPermBuffer;
		private int[]					computeResultData;
		private ComputeBuffer			computeResultBuffer;

		private int						_width;
		private int						_height;

		private PlanetNoise				_surfaceParams;
		private PlanetNoise				_landParams;
		private Color[]					_landColour;
		private PlanetNoise[]			_landColourNoise;
		private float					_cityReach;
		private float					_cityHeight;
		private Color					_waterColour0;
		private Color					_waterColour1;
		private float					_waterLevel;
		private float					_waterSpecular;
		private float					_waterFalloff;
		private Color					_iceColour;
		private float					_iceReach;
		private float					_iceHeight;
		private float					_shadowRange;
		private float					_shadowStrength;
		private float					_normalScale;
		private int						_diffuseKernel				= 0;

		private System.Random			_cityRnd;
		private List< CitySplatData >	_citySplatList;
		private float					_cityMaxIntensity;
		private float					_cityFalloff;
		private int						_diffuseStartIndexC;
		private int						_tileCountX;
		private int						_tileCountY;
		private bool					isDoingCityWork;

		private PlanetNoise				_cloudParams;
		private int						_cloudWorleyNoiseOctaves;
		private float					_cloudWorleyNoiseFrequency;
		private float					_cloudWorleyNoiseAmplitude;
		private float					_cloudShadowRange;
		private float					_cloudShadowStrength;
		private Color					_cloudColour0;
		private Color					_cloudColour1;
		private float					_cloudSpin;
		private float					_cloudNormalScale;
		private bool					_preMultiplyAlpha;
		private int						_cloudKernel				= 0;
		
		private Thread[]				_worker;
		private int						_jobCount;
		private int						_jobStartedCount;
		private int						_jobCompletedCount;
		private int[]					_jobList;
		private Object					lockObj						= new Object();

		private struct CityTileRect
		{
			public int minx;
			public int miny;
			public int maxx;
			public int maxy;
		}

		private struct CitySplatData
		{
			public int		x;
			public int		y;
			public float	spread;
			public float	intensity;
		}

		private bool doWork( WorkAction workAction )
		{
			int job = -1;

			while( true )
			{
				lock( lockObj )
				{
					if( _jobStartedCount < _jobCount )
					{
						job = _jobList[ _jobStartedCount ];
						++_jobStartedCount;
					}

					break;
				}
			}

			if( job >= 0 )
			{
				workAction( job, _jobCount );

				while( true )
				{
					lock( lockObj )
					{
						++_jobCompletedCount;
						break;
					}
				}

				return true;
			}

			return false;
		}

		private void diffuseNoiseWork( int jobId, int jobCount )
		{
			int		pixelCount		= _width * _height;
			int		pixelsPerJob	= Mathf.FloorToInt( pixelCount / ( float ) jobCount );
			
			int		startIndex		= pixelsPerJob * jobId;
			int		endIndex		= ( jobId == jobCount - 1 ? pixelCount : pixelsPerJob * ( jobId + 1 ) );

			Vector3	n				= Vector3.zero;
			Vector3	cr				= Vector3.zero;
			
			for( int i = startIndex; i < endIndex; ++i )
			{
				int y		= Mathf.FloorToInt( i / ( float ) _width );
				int x		= i - ( _width * y );
				
				int index	= i * 4;
				int hIndex	= ( pixelCount * 4 ) + index;
				int cIndex	= ( pixelCount * 8 ) + i;

				float inW	= 1f / ( float ) ( _width - 1f );
				float inH	= 1f / ( float ) ( _height - 1f );

				float u		= x * inW;
				float v		= y * inH;

				Vector3 p	= getPointOnSphere( u, v );
				float h		= getSurfaceHeight( p, _surfaceParams, perlinNoiseSurface );

				float dr	= 1f;

				if( h > _waterLevel )
				{
					Color c		= _iceColour;
					float ir	= _iceReach - ( h * 0.1f );
					float ao	= 1f;

#if true
					//land colour
					if( h <= _iceHeight && v > ir && v < 1f - ir )
					{
						Color c0	= getSurfaceColour( p, _landColourNoise[ 0 ], _landColour[ 0 ], _landColour[ 1 ], perlinNoiseLandColour01 );
						Color c1	= getSurfaceColour( p, _landColourNoise[ 1 ], _landColour[ 2 ], _landColour[ 3 ], perlinNoiseLandColour23 );
						c			= getSurfaceColour( p, _landParams, c0, c1, perlinNoiseLand );
					}
#endif

#if true
					//height and normals
					Vector3 px	= getPointOnSphere( ( x + dr ) * inW, v );
					Vector3 py	= getPointOnSphere( u, ( y + dr ) * inH );

					float hx	= getSurfaceHeight( px, _surfaceParams, perlinNoiseSurface );
					float hy	= getSurfaceHeight( py, _surfaceParams, perlinNoiseSurface );

					n.x			= dr * inW;
					n.y			= 0;
					n.z			= hx - h;
					n			*= 1000f;			//prevents rounding errors in cross product below
					cr.x		= 0;
					cr.y		= dr * inH;
					cr.z		= hy - h;
					cr			*= 1000f;			//prevents rounding errors in cross product below

					n			= Vector3.Cross( n, cr ).normalized;

					ao			= 1f - ( ( 1f - Mathf.Clamp01( Vector3.Dot( n, Vector3.forward ) * _shadowRange ) ) * _shadowStrength );

					n			= ( n * _normalScale * 0.5f ) + ( Vector3.one * 0.5f );
					h			= ( h - _waterLevel ) / ( 1f - _waterLevel );

					diffuseData[ hIndex ]		= ( byte ) Mathf.RoundToInt( n.x * 255 );
					diffuseData[ hIndex + 1 ]	= ( byte ) Mathf.RoundToInt( n.y * 255 );
					diffuseData[ hIndex + 2 ]	= ( byte ) Mathf.RoundToInt( n.z * 255 );
					diffuseData[ hIndex + 3 ]	= ( byte ) Mathf.RoundToInt( h * 255 );
#endif

#if true
					//set land colour here after calculating ao
					diffuseData[ index ]		= ( byte ) Mathf.RoundToInt( c.r * 255 * ao );
					diffuseData[ index + 1 ]	= ( byte ) Mathf.RoundToInt( c.g * 255 * ao );
					diffuseData[ index + 2 ]	= ( byte ) Mathf.RoundToInt( c.b * 255 * ao );
					diffuseData[ index + 3 ]	= 0;
#endif

#if true
					//city illumination
					diffuseData[ cIndex ] = 0;

					if( h < _cityHeight )
					{
						float cityLowerLimit	= 0.5f - _cityReach;
						float cityUpperLimit	= 0.5f + _cityReach;
						float cityDivisor		= cityLowerLimit;
						
						if( v < 0.5f - _cityReach )
							diffuseData[ cIndex ] = ( byte ) Mathf.RoundToInt( 255 * Mathf.Pow( v / cityDivisor, 3f ) );
						else if( v > 0.5f + _cityReach )
							diffuseData[ cIndex ] = ( byte ) Mathf.RoundToInt( 255 * Mathf.Pow( ( cityLowerLimit - ( v - cityUpperLimit ) ) / cityDivisor, 3f ) );
						else
							diffuseData[ cIndex ] = 255;
					}
#endif
				}
				else
				{
					Color c		= _iceColour;
					float ir	= _iceReach - ( h * 0.2f );
					float spec	= 0;
					float ao	= 1f;

#if true
					//water colour
					if( h <= _iceHeight && v > ir && v < 1f - ir )
					{
						float s		= Mathf.Pow( h / _waterLevel, _waterFalloff );
						float q1	= 6f * Mathf.Pow( s, 5f ) - 15f * Mathf.Pow( s, 4f ) + 10f * Mathf.Pow( s, 3f );
						float q0	= 1f - q1;

						c			= new Color( ( _waterColour0.r * q0 ) + ( _waterColour1.r * q1 ), ( _waterColour0.g * q0 ) + ( _waterColour1.g * q1 ),
													( _waterColour0.b * q0 ) + ( _waterColour1.b * q1 ) );

						spec		= _waterSpecular;
					}
#endif

#if true
					//height and normals for polar caps, none for water
					if( h <= _iceHeight && v > ir && v < 1f - ir )
					{
						diffuseData[ hIndex ]		= 128;
						diffuseData[ hIndex + 1 ]	= 128;
						diffuseData[ hIndex + 2 ]	= 255;
						diffuseData[ hIndex + 3 ]	= 0;
					}
					else
					{
						h			= Mathf.Abs( _waterLevel - h ) + _waterLevel;					//invert height below water to height above water

						Vector3 px	= getPointOnSphere( ( x + dr ) * inW, v );
						Vector3 py	= getPointOnSphere( u, ( y + dr ) * inH );

						float hx	= Mathf.Abs( _waterLevel - getSurfaceHeight( px, _surfaceParams, perlinNoiseSurface ) ) + _waterLevel;
						float hy	= Mathf.Abs( _waterLevel - getSurfaceHeight( py, _surfaceParams, perlinNoiseSurface ) ) + _waterLevel;

						n.x			= dr * inW;
						n.y			= 0;
						n.z			= hx - h;
						n			*= 1000f;			//prevents rounding errors in cross product below
						cr.x		= 0;
						cr.y		= dr * inH;
						cr.z		= hy - h;
						cr			*= 1000f;			//prevents rounding errors in cross product below

						n			= Vector3.Cross( n, cr ).normalized;
						n			= ( n * _normalScale * 0.5f ) + ( Vector3.one * 0.5f );
						h			= ( h - _waterLevel ) / ( 1f - _waterLevel );

						diffuseData[ hIndex ]		= ( byte ) Mathf.RoundToInt( n.x * 255 );
						diffuseData[ hIndex + 1 ]	= ( byte ) Mathf.RoundToInt( n.y * 255 );
						diffuseData[ hIndex + 2 ]	= ( byte ) Mathf.RoundToInt( n.z * 255 );
						diffuseData[ hIndex + 3 ]	= ( byte ) Mathf.RoundToInt( h * 255 );
					}
#endif

#if true
					//set colour here after calculating ao
					diffuseData[ index ]		= ( byte ) Mathf.RoundToInt( c.r * 255 * ao );
					diffuseData[ index + 1 ]	= ( byte ) Mathf.RoundToInt( c.g * 255 * ao );
					diffuseData[ index + 2 ]	= ( byte ) Mathf.RoundToInt( c.b * 255 * ao );
					diffuseData[ index + 3 ]	= ( byte ) Mathf.RoundToInt( spec * 255 );
#endif

#if true
					//no city illumination
					diffuseData[ cIndex ] = 0;
#endif
				}
			}
		}

		private void cityNoiseWork( int jobId, int jobCount )
		{
			int tileSize			= ( _width / _tileCountX );

			CityTileRect tileRect	= new CityTileRect();
			tileRect.minx			= ( jobId % _tileCountX ) * tileSize;
			tileRect.maxx			= tileRect.minx + tileSize;
			tileRect.miny			= ( jobId / _tileCountX ) * tileSize;
			tileRect.maxy			= tileRect.miny + tileSize;

#if true
			for( int i = 0; i < _citySplatList.Count; ++i )
			{
				CitySplatData citySplat = _citySplatList[ i ];
				
				int intSpread			= Mathf.RoundToInt( citySplat.spread );

				float spreadTop			= ( 0.5f - getSphericalDistortion( citySplat.y - intSpread ) ) * 2f;
				float spreadBottom		= ( 0.5f - getSphericalDistortion( citySplat.y + intSpread ) ) * 2f;

				float spreadDistortion;

				if( spreadTop <= 0f || spreadBottom <= 0f )
					spreadDistortion = _width * 0.5f;
				else
					spreadDistortion = Mathf.Min( citySplat.spread / Mathf.Min( spreadTop, spreadBottom ), _width * 0.5f );

				int intSpreadDistortion	= Mathf.RoundToInt( spreadDistortion );

				float expFalloff		= 1f / Mathf.Exp( _cityFalloff );
				float invSpread			= 1f / citySplat.spread;

				int minx				= Mathf.Max( tileRect.minx, citySplat.x - intSpreadDistortion );
				int maxx				= Mathf.Min( tileRect.maxx, citySplat.x + intSpreadDistortion + 1 );

				int miny				= Mathf.Max( tileRect.miny, citySplat.y - intSpread );
				int maxy				= Mathf.Min( tileRect.maxy, citySplat.y + intSpread + 1 );

				float referenceWidth	= 8192f;
				float referenceHeight	= referenceWidth * 0.5f;

				for( int a = minx; a < maxx; ++a )
				{
					float iDistortion	= ( ( citySplat.x - a ) / spreadDistortion ) * citySplat.spread + citySplat.x;
					float xi			= ( citySplat.x - iDistortion ) * ( citySplat.x - iDistortion );
					xi					= ( xi / _width ) * referenceWidth;

					for( int b = miny; b < maxy; ++b )
					{
						float xj				= ( citySplat.y - b ) * ( citySplat.y - b );
						xj						= ( xj / _height ) * referenceHeight;
						float dist				= Mathf.Max( citySplat.spread - Mathf.Sqrt( xi + xj ), 0f ) * invSpread;

						int index				= getIndex( a, b );

						float diffuseMultiplier	= diffuseData[ _diffuseStartIndexC + index ] / 255f;
						
						float val				= cityData[ index ] / diffuseMultiplier;
						val						+= ( ( citySplat.intensity * dist ) * expFalloff ) * dist;
					
						cityData[ index ]		= Mathf.Min( val, _cityMaxIntensity ) * diffuseMultiplier;
					}
				}
			}
#endif
		}

		private void cloudNoiseWork( int jobId, int jobCount )
		{
			int		pixelCount		= _width * _height;
			int		pixelsPerJob	= Mathf.FloorToInt( pixelCount / ( float ) jobCount );

			int		startIndex		= pixelsPerJob * jobId;
			int		endIndex		= ( jobId == jobCount - 1 ? pixelCount : pixelsPerJob * ( jobId + 1 ) );
			
			Vector3	n				= Vector3.zero;
			Vector3	cr				= Vector3.zero;
			
			for( int i = startIndex; i < endIndex; ++i )
			{
				int y		= Mathf.FloorToInt( i / ( float ) _width );
				int x		= i - ( _width * y );
				
				int index	= i * 4;
				int hIndex	= ( pixelCount * 4 ) + index;

				float inW	= 1f / ( float ) ( _width - 1f );
				float inH	= 1f / ( float ) ( _height - 1f );

				float u		= x * inW;
				float v		= y * inH;

				float dr	= 1f;

				Vector3 p	= getPointOnSphere( u, v );
				p.x			/= _cloudSpin;
				p.y			/= _cloudSpin;
				float h		= 1f;

				float a		= 1f;

#if true
				//colour
				Color c		= getSurfaceColour( p, _cloudParams, _cloudColour0, _cloudColour1, perlinNoiseCloud );
				a			= c.a * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( p * _cloudWorleyNoiseFrequency, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) );
				a			+= c.a * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( p * _cloudWorleyNoiseFrequency * 2, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				a			+= c.a * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( p * _cloudWorleyNoiseFrequency * 4, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				h			= a * 0.5f;
				a			= Mathf.Clamp01( a );
				c			*= ( _preMultiplyAlpha ? a : 1f );

				cloudData[ index ]		= ( byte ) Mathf.RoundToInt( c.r * 255 );
				cloudData[ index + 1 ]	= ( byte ) Mathf.RoundToInt( c.g * 255 );
				cloudData[ index + 2 ]	= ( byte ) Mathf.RoundToInt( c.b * 255 );
				cloudData[ index + 3 ]	= ( byte ) Mathf.RoundToInt( a * 255 * Mathf.Clamp01( c.r + c.g + c.b ) );
#endif

#if true
				//height and normals
				Vector3 px	= getPointOnSphere( ( x + dr ) * inW, v );
				px.x		/= _cloudSpin;
				px.y		/= _cloudSpin;
				Vector3 py	= getPointOnSphere( u, ( y + dr ) * inH );
				py.x		/= _cloudSpin;
				py.y		/= _cloudSpin;

				float hx	= getSurfaceHeight( px, _cloudParams, perlinNoiseCloud );
				float ax	= hx * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( px * _cloudWorleyNoiseFrequency, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) );
				ax			+= hx * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( px * _cloudWorleyNoiseFrequency * 2, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				ax			+= hx * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( px * _cloudWorleyNoiseFrequency * 4, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				hx			= ax * 0.5f;

				float hy	= getSurfaceHeight( py, _cloudParams, perlinNoiseCloud );
				float ay	= hy * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( py * _cloudWorleyNoiseFrequency, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) );
				ay			+= hy * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( py * _cloudWorleyNoiseFrequency * 2, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				ay			+= hy * Mathf.Clamp01( 1f - worleyNoiseCloud.getOctave( py * _cloudWorleyNoiseFrequency * 4, _cloudWorleyNoiseOctaves, _cloudWorleyNoiseAmplitude ) ) * 0.5f;
				hy			= ay * 0.5f;

				n.x			= dr * inW;
				n.y			= 0;
				n.z			= hx - h;
				n			*= 1000f;			//prevents rounding errors in cross product below
				cr.x		= 0;
				cr.y		= dr * inH;
				cr.z		= hy - h;
				cr			*= 1000f;			//prevents rounding errors in cross product below
				n			= Vector3.Cross( n, cr ).normalized;
				
				n			= ( n * a * _cloudNormalScale * 0.5f ) + ( Vector3.one * 0.5f );
				
				cloudData[ hIndex ]		= ( byte ) Mathf.RoundToInt( n.x * 255 );
				cloudData[ hIndex + 1 ]	= ( byte ) Mathf.RoundToInt( n.y * 255 );
				cloudData[ hIndex + 2 ]	= ( byte ) Mathf.RoundToInt( n.z * 255 );
				cloudData[ hIndex + 3 ]	= ( byte ) Mathf.RoundToInt( h * 255 );
#endif
			}
		}

		private float getSurfaceHeight( Vector3 pos, PlanetNoise noiseParams, PlanetNoisePerlin perlinNoise )
		{
			float result = sample( pos, noiseParams, perlinNoise );
			return result;
		}
		
		private Color getSurfaceColour( Vector3 pos, PlanetNoise noiseParams, Color colour0, Color colour1, PlanetNoisePerlin perlinNoise )
		{
			float c			= sample( pos, noiseParams, perlinNoise );
			Color result	= Color.Lerp( colour0, colour1, 1f - c );
			return result;
		}

		private float sample( Vector3 pos, PlanetNoise noiseParams, PlanetNoisePerlin perlinNoise )
		{
			float result	= 0f;
			float offset	= 0f;

			if( noiseParams.smearOctaves > 0 )
			{
				offset = perlinNoise.getOctave( pos / noiseParams.smearScale, noiseParams.smearOctaves );
				offset = Mathf.Pow( offset, noiseParams.smearFalloff );
				offset *= noiseParams.smearIntensity;
			}

			result = perlinNoise.getNormalisedOctave( ( pos / noiseParams.scale ) + ( Vector3.one * offset ), noiseParams.octaves );

			if( noiseParams.ridginess > 0f )
			{
				float ridge	= perlinNoise.getNormalisedOctave( ( pos / noiseParams.scale ) + new Vector3( offset, offset, offset + 11f ), noiseParams.octaves );
				result		= ( noiseParams.ridginess * ( 1f - ( Mathf.Abs( ridge - 0.5f ) * 2f ) ) ) + ( ( 1f - noiseParams.ridginess ) * result );
			}

			result = Mathf.Pow( result, noiseParams.falloff );
			result = Mathf.Clamp01( result * noiseParams.intensity );

			return result;
		}
		
		private Vector3 getPointOnSphere( float u, float v )
		{
			float s = Mathf.PI * u * 2f;
			float t = Mathf.PI * v;

			Vector3 result = new Vector3( Mathf.Sin( t ) * Mathf.Cos( s ), Mathf.Sin( t ) * Mathf.Sin( s ), Mathf.Cos( t ) );
			return result;
		}

		//0 <= result <= 0.5, 0 = equator
		private float getSphericalDistortion( int y )
		{
			y				= ( y + _height ) % _height;
			float result	= Mathf.Abs( ( y / ( float ) _height ) - 0.5f );
			return result;
		}

		private int getIndex( int x, int y )
		{
			while( x < 0 || x >= _width )
				x = ( x + _width ) % _width;

			while( y < 0 || y >= _height )
				y = ( y + _height ) % _height;

			return x + ( y * _width );
		}
	}
}