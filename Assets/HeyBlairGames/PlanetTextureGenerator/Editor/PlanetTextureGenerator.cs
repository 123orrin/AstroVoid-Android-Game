using UnityEngine;
using UnityEditor;

using System.IO;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetTextureGenerator
	{
		public const int	previewWidth	= 256;
		public const int	previewHeight	= 128;

		public PlanetAsset	_target;

		public Texture2D	diffusePreview;
		public Texture2D	normalPreview;
		public Texture2D	illuminationPreview;
		public Texture2D	cloudPreview;

		public PlanetTextureGenerator()
		{
			noiseGenerator					= new PlanetNoiseGenerator();
			
			diffusePreview					= new Texture2D( previewWidth, previewHeight, TextureFormat.ARGB32, false );
			diffusePreview.hideFlags		= HideFlags.HideAndDontSave;
			normalPreview					= new Texture2D( previewWidth, previewHeight, TextureFormat.ARGB32, false );
			normalPreview.hideFlags			= HideFlags.HideAndDontSave;
			illuminationPreview				= new Texture2D( previewWidth, previewHeight, TextureFormat.ARGB32, false );
			illuminationPreview.hideFlags	= HideFlags.HideAndDontSave;
			cloudPreview					= new Texture2D( previewWidth, previewHeight, TextureFormat.ARGB32, false );
			cloudPreview.hideFlags			= HideFlags.HideAndDontSave;
		}

		//blocks main thread to stop user modifying parameters while updating preview textures
		public void updatePreview()
		{
			updatePerlinNoise();

			//create diffuse data
			noiseGenerator.generateDiffuseNoise( previewWidth, previewHeight,
													_target.surfaceNoise,
													_target.landNoise,
													new Color[] { _target.landColour0, _target.landColour1, _target.landColour2, _target.landColour3 },
													new PlanetNoise[] { _target.landColourNoise01, _target.landColourNoise23 },
													_target.cityReach,
													_target.cityHeight,
													_target.waterColour0,
													_target.waterColour1,
													_target.waterLevel,
													_target.waterSpecular,
													_target.waterFalloff,
													_target.iceColour,
													_target.iceReach,
													_target.iceHeight,
													_target.shadowRange,
													_target.shadowStrength,
													_target.normalScale );
			
			do
			{
				EditorUtility.DisplayProgressBar( "Ground Map Generation", "Creating Ground Data", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doDiffuseWork() );

			noiseGenerator.waitForGenerator();
			
#if true
			//ground map
			EditorUtility.DisplayProgressBar( "Ground Map Generation", "Creating Ground Data", 1f );
			updateTexture( diffusePreview, noiseGenerator.diffuseData );
#endif
		
#if true
			//normal map
			EditorUtility.DisplayProgressBar( "Normal Map Generation", "Creating Normal Data", 1f );
			updateTexture( normalPreview, noiseGenerator.diffuseData, previewWidth * previewHeight * 4 );
#endif

#if true
			//illumination map
			noiseGenerator.generateCityNoise( previewWidth, previewHeight,
												_target.cityCount,
												_target.cityMultiplier,
												_target.cityDropoff,
												_target.cityDepth, 
												_target.citySpread,
												_target.cityIntensity,
												_target.maxCityIntensity,
												_target.cityFalloff );

			do
			{
				EditorUtility.DisplayProgressBar( "Illumination Map Generation", "Creating Illumination Data", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doCityWork() );

			noiseGenerator.waitForGenerator();

			EditorUtility.DisplayProgressBar( "Illumination Map Generation", "Creating Illumination Data", 1f );
			updateIlluminationTexture( illuminationPreview, noiseGenerator.cityData, noiseGenerator.diffuseData, false );
			noiseGenerator.cityData = null;
#endif

			noiseGenerator.diffuseData = null;
		
#if true
			//cloud texture
			noiseGenerator.generateCloudNoise( previewWidth, previewHeight,
												_target.cloudNoise,
												_target.cloudWorleyNoiseOctaves,
												_target.cloudWorleyNoiseFrequency,
												_target.cloudWorleyNoiseAmplitude,
												_target.cloudShadowRange,
												_target.cloudShadowStrength,
												_target.cloudColour0,
												_target.cloudColour1,
												_target.cloudSpin,
												_target.cloudNormalScale,
												true );
			
			do
			{
				EditorUtility.DisplayProgressBar( "Cloud Texture Generation", "Creating Cloud Data", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doCloudWork() );

			noiseGenerator.waitForGenerator();
			
			updateTexture( cloudPreview, noiseGenerator.cloudData );
			noiseGenerator.cloudData = null;
#endif
			
			EditorUtility.ClearProgressBar();
		}

		//blocks main thread to stop user modifying parameters during bake
		public void bakeGroundTextures()
		{
			updatePerlinNoise();

			string assetDirectory	= AssetDatabase.GetAssetPath( _target );
			assetDirectory			= assetDirectory.Substring( 0, assetDirectory.LastIndexOf( "/" ) + 1 );
			
			byte[] data;
			
			int width	= 1 << ( _target.size + 8 );
			int height	= width / 2;
			
			Texture2D outputTexture	= new Texture2D( width, height, TextureFormat.ARGB32, false );
			
			//create diffuse data
			noiseGenerator.generateDiffuseNoise( width, height,
													_target.surfaceNoise,
													_target.landNoise,
													new Color[] { _target.landColour0, _target.landColour1, _target.landColour2, _target.landColour3 },
													new PlanetNoise[] { _target.landColourNoise01, _target.landColourNoise23 },
													_target.cityReach,
													_target.cityHeight,
													_target.waterColour0,
													_target.waterColour1,
													_target.waterLevel,
													_target.waterSpecular,
													_target.waterFalloff,
													_target.iceColour,
													_target.iceReach,
													_target.iceHeight,
													_target.shadowRange,
													_target.shadowStrength,
													_target.normalScale );
			
			do
			{
				EditorUtility.DisplayProgressBar( "Ground Map Generation", "Creating Ground Map", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doDiffuseWork() );

			noiseGenerator.waitForGenerator();
			
#if true			
			EditorUtility.DisplayProgressBar( "Ground Map Generation", "Creating Ground Map", 1 );
			updateTexture( outputTexture, noiseGenerator.diffuseData );
			data = outputTexture.EncodeToPNG();
				
			EditorUtility.DisplayProgressBar( "Ground Map Generation", "Saving Ground Map", 1 );
			File.WriteAllBytes( assetDirectory + "GroundMap.png", data );
#endif
			
#if true
			//normal map
			EditorUtility.DisplayProgressBar( "Normal Map Generation", "Creating Normal Data", 1 );
			updateTexture( outputTexture, noiseGenerator.diffuseData, width * height * 4 );
			data = outputTexture.EncodeToPNG();
			
			EditorUtility.DisplayProgressBar( "Normal Map Generation", "Saving Normal Map", 1 );
			File.WriteAllBytes( assetDirectory + "NormalMap.png", data );
#endif
			
#if true
			//height map
			if( !_target.combineHeightWithCity )
			{
				EditorUtility.DisplayProgressBar( "Height Map Generation", "Creating Height Data", 1 );
				updateHeightTexture( outputTexture, noiseGenerator.diffuseData );
				data = outputTexture.EncodeToPNG();
				
				EditorUtility.DisplayProgressBar( "Height Map Generation", "Saving Height Map", 1 );
				File.WriteAllBytes( assetDirectory + "HeightMap.png", data );
			}
#endif
		
#if true
			//illumination map
			noiseGenerator.generateCityNoise( outputTexture.width, outputTexture.height,
												_target.cityCount,
												_target.cityMultiplier,
												_target.cityDropoff,
												_target.cityDepth, 
												_target.citySpread,
												_target.cityIntensity,
												_target.maxCityIntensity,
												_target.cityFalloff );

			do
			{
				EditorUtility.DisplayProgressBar( "Illumination Map Generation", "Creating Illumination Data", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doCityWork() );

			noiseGenerator.waitForGenerator();

			EditorUtility.DisplayProgressBar( "Illumination Map Generation", "Creating Illumination Data", 1f );
			updateIlluminationTexture( outputTexture, noiseGenerator.cityData, noiseGenerator.diffuseData, _target.combineHeightWithCity );
			data = outputTexture.EncodeToPNG();
			
			EditorUtility.DisplayProgressBar( "Illumination Map Generation", "Saving Illumination Map", 1 );
			
			File.WriteAllBytes( assetDirectory + "IlluminationMap.png", data );

			noiseGenerator.cityData = null;
#endif
			
			noiseGenerator.diffuseData = null;
			
			Object.DestroyImmediate( outputTexture );
			
			EditorUtility.ClearProgressBar();
		
#if true
			AssetDatabase.ImportAsset( assetDirectory + "GroundMap.png" );
			AssetDatabase.ImportAsset( assetDirectory + "NormalMap.png" );
			AssetDatabase.ImportAsset( assetDirectory + "IlluminationMap.png" );
			
			if( !_target.combineHeightWithCity )
				AssetDatabase.ImportAsset( assetDirectory + "HeightMap.png" );
#endif
		}

		public void bakeCloudTextures()
		{
			updatePerlinNoise();

			string assetDirectory	= AssetDatabase.GetAssetPath( _target );
			assetDirectory			= assetDirectory.Substring( 0, assetDirectory.LastIndexOf( "/" ) + 1 );
			
			byte[] data;
			
			int width	= 1 << ( _target.size + 8 );
			int height	= width / 2;
			
			Texture2D outputTexture	= new Texture2D( width, height, TextureFormat.ARGB32, false );

			//create cloud data
			noiseGenerator.generateCloudNoise( width, height,
												_target.cloudNoise,
												_target.cloudWorleyNoiseOctaves,
												_target.cloudWorleyNoiseFrequency,
												_target.cloudWorleyNoiseAmplitude,
												_target.cloudShadowRange,
												_target.cloudShadowStrength,
												_target.cloudColour0,
												_target.cloudColour1,
												_target.cloudSpin,
												_target.cloudNormalScale,
												true );
			
			do
			{
				EditorUtility.DisplayProgressBar( "Cloud Map Generation", "Creating Cloud Map", noiseGenerator.getProgress() );
			}
			while( noiseGenerator.doCloudWork() );

			noiseGenerator.waitForGenerator();
			
#if true
			EditorUtility.DisplayProgressBar( "Cloud Map Generation", "Creating Cloud Map", 1 );
			updateTexture( outputTexture, noiseGenerator.cloudData );
			data = outputTexture.EncodeToPNG();
				
			EditorUtility.DisplayProgressBar( "Cloud Map Generation", "Saving Cloud Map", 1 );
			File.WriteAllBytes( assetDirectory + "CloudMap.png", data );
#endif

#if true
			//cloud normal map
			EditorUtility.DisplayProgressBar( "Cloud Normal Map Generation", "Creating Normal Data", 1 );
			updateTexture( outputTexture, noiseGenerator.cloudData, width * height * 4 );
			data = outputTexture.EncodeToPNG();
			
			EditorUtility.DisplayProgressBar( "Cloud Normal Map Generation", "Saving Normal Map", 1 );
			File.WriteAllBytes( assetDirectory + "CloudNormalMap.png", data );
#endif
			
			noiseGenerator.cloudData = null;

			Object.DestroyImmediate( outputTexture );
			
			EditorUtility.ClearProgressBar();
			
#if true
			AssetDatabase.ImportAsset( assetDirectory + "CloudMap.png" );
			AssetDatabase.ImportAsset( assetDirectory + "CloudNormalMap.png" );
#endif
		}


		private PlanetNoiseGenerator noiseGenerator;
		
		private void updateTexture( Texture2D texture, byte[] data, int startIndex = 0 )
		{
			int pixelCount		= texture.width * texture.height;
			
			Color32[] colour	= new Color32[ pixelCount ];
			
			for( int i = 0; i < pixelCount; ++i )
			{
				int index = ( i * 4 ) + startIndex;
				
				colour[ i ] = new Color32( data[ index ], data[ index + 1 ], data[ index + 2 ], data[ index + 3 ] );
			}
			
			texture.SetPixels32( colour, 0 );
			texture.Apply( false );
			
			colour = null;
		}
		
		private void updateIlluminationTexture( Texture2D texture, float[] data, byte[] diffuseData, bool withHeight )
		{
			int pixelCount		= texture.width * texture.height;
			int startIndexC		= 0;
			int startIndexH		= pixelCount * 4;
			
			Color32[] colour	= new Color32[ pixelCount ];
			
			for( int i = 0; i < pixelCount; ++i )
			{
				int indexC		= ( i * 1 ) + startIndexC;
				int indexH		= ( i * 4 ) + startIndexH + 3;
				
				colour[ i ]		= _target.cityColour * data[ indexC ];
				colour[ i ].a	= withHeight ? diffuseData[ indexH ] : ( byte ) 0;
			}
			
			texture.SetPixels32( colour, 0 );
			texture.Apply( false );
			
			colour = null;
		}
		
		private void updateHeightTexture( Texture2D texture, byte[] data )
		{
			int pixelCount		= texture.width * texture.height;
			int startIndex		= pixelCount * 4;
			
			Color32[] colour	= new Color32[ pixelCount ];
			
			for( int i = 0; i < pixelCount; ++i )
			{
				int index	= ( i * 4 ) + startIndex;
				byte h		= data[ index + 3 ];
				
				colour[ i ] = new Color32( h, h, h, h );
			}
			
			texture.SetPixels32( colour, 0 );
			texture.Apply( false );
			
			colour = null;
		}

		private void updatePerlinNoise()
		{
			noiseGenerator.createNoise( _target.seedSurface, _target.seedLand, _target.seedLandColour01, _target.seedLandColour23, _target.seedCity, _target.seedCloud );
		}
	}
}