using UnityEngine;
using UnityEditor;

using System.IO;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	[ CustomEditor( typeof( PlanetAsset ) ) ]
	public class PlanetComponentEditor : UnityEditor.Editor
	{
		[ MenuItem( "Assets/Create/HeyBlairGames/Planet Asset" ) ]
		static public void CreateAsset()
		{
			PlanetAsset asset = ScriptableObject.CreateInstance< PlanetAsset >();
	        
			string path = AssetDatabase.GetAssetPath( Selection.activeObject );

			if( path == "" )
				path = "Assets";
			else if( Path.GetExtension( path ) != "" )
				path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
	        
			string assetName		= typeof( PlanetAsset ).ToString();
			assetName				= "/New " + assetName.Substring( assetName.LastIndexOf( "." ) + 1 );
			string assetPathAndName	= AssetDatabase.GenerateUniqueAssetPath( path + assetName + ".asset" );
	        
			AssetDatabase.CreateAsset( asset, assetPathAndName );
	        
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}

		public void OnEnable()
		{
			if( target != _target )
			{
				_target = target as PlanetAsset;

				if( textureGenerator == null )
				{
					textureGenerator			= new PlanetTextureGenerator();
					textureGenerator._target	= _target;
				}
			}
		}
		
		override public void OnInspectorGUI()
		{
			showParameterGUI();
			
	        if( GUI.changed )
				EditorUtility.SetDirty( _target );
		}
		
		
		private PlanetAsset				_target;
		private PlanetTextureGenerator	textureGenerator;

		private bool					showingSurfaceNoise		= false;
		private bool					showingLandNoise		= false;
		private bool					showingLandColourNoise0	= false;
		private bool					showingLandColourNoise1	= false;
		private bool					showingCloudNoise		= false;
		
		//width of textures, height is half of width
		private string[]				sizeOptions				= new string[] { "256", "512", "1024", "2048", "4096", "8192" };

		private void updatePreviewIfAvailable()
		{
			EditorWindow.FocusWindowIfItsOpen< PlanetPreviewWindow >();
			EditorWindow ew = EditorWindow.focusedWindow;

			if( ew is PlanetPreviewWindow )
				( ( PlanetPreviewWindow ) ew ).updatePreview();
		}

		private void showParameterGUI()
		{
			if( GUILayout.Button( "Randomise All", EditorStyles.miniButton ) )
			{
				Undo.RecordObject( _target, "Randomise All" );
				_target.randomise();
				EditorApplication.delayCall += updatePreviewIfAvailable;
			}

			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise Surface Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise Surface Seed" );
					_target.seedSurface = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			showingSurfaceNoise = showGUINoise( _target.surfaceNoise, "Surface Noise", showingSurfaceNoise );


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise Land Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise Land Seed" );
					_target.seedLand = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			showingLandNoise	= showGUINoise( _target.landNoise, "Land Noise", showingLandNoise );
			_target.landColour0	= showColourPicker( "Land Colour 0",	_target.landColour0 );
			_target.landColour1	= showColourPicker( "Land Colour 1",	_target.landColour1 );
			_target.landColour2	= showColourPicker( "Land Colour 2",	_target.landColour2 );
			_target.landColour3	= showColourPicker( "Land Colour 3",	_target.landColour3 );

			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise Land Colour 01 Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise Land Colour 01 Seed" );
					_target.seedLandColour01 = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();

			showingLandColourNoise0 = showGUINoise( _target.landColourNoise01, "Land Colour Noise 0", showingLandColourNoise0 );

			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise Land Colour 23 Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise Land Colour 23 Seed" );
					_target.seedLandColour23 = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();

			showingLandColourNoise1 = showGUINoise( _target.landColourNoise23, "Land Colour Noise 1", showingLandColourNoise1 );


			EditorGUILayout.Space();

			
			_target.waterColour0	= showColourPicker( "Water Colour 0",	_target.waterColour0 );
			_target.waterColour1	= showColourPicker( "Water Colour 1",	_target.waterColour1 );
			_target.waterLevel		= showGUISlider( "Water Level",			_target.waterLevel,		0f,		1f );
			_target.waterSpecular	= showGUISlider( "Water Specular",		_target.waterSpecular,	0f,		1f );
			_target.waterFalloff	= showGUISlider( "Water Falloff",		_target.waterFalloff,	0.1f,	20f );


			EditorGUILayout.Space();


			_target.iceColour		= showColourPicker( "Ice Colour",		_target.iceColour );
			_target.iceReach		= showGUISlider( "Ice Reach",			_target.iceReach,		0f,		1f );
			_target.iceHeight		= showGUISlider( "Ice Height",			_target.iceHeight,		0f,		1f );


			EditorGUILayout.Space();


			_target.shadowRange		= showGUISlider( "Shadow Range",		_target.shadowRange,	0f,		100f );
			_target.shadowStrength	= showGUISlider( "Shadow Strength",		_target.shadowStrength,	0f,		1f );


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise City Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise City Seed" );
					_target.seedCity = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();

			_target.cityReach			= showGUISlider( "City Reach",			_target.cityReach,			0f,		1f );
			_target.cityHeight			= showGUISlider( "City Height",			_target.cityHeight,			0f,		1f );
			_target.cityColour			= showColourPicker( "City Colour",		_target.cityColour );
			_target.cityCount			= showGUISlider( "City Count",			_target.cityCount,			0,		256 );
			_target.cityMultiplier		= showGUISlider( "City Multiplier",		_target.cityMultiplier,		1,		10 );
			_target.cityDropoff			= showGUISlider( "City Dropoff",		_target.cityDropoff,		1,		10 );
			_target.cityDepth			= showGUISlider( "City Depth",			_target.cityDepth,			1,		10 );
			_target.citySpread			= showGUISlider( "City Spread",			_target.citySpread,			0.01f,	8f );
			_target.cityIntensity		= showGUISlider( "City Intensity",		_target.cityIntensity,		4f,		256f );
			_target.maxCityIntensity	= showGUISlider( "Max City Intensity",	_target.maxCityIntensity,	1f,		128f );
			_target.cityFalloff			= showGUISlider( "City Falloff",		_target.cityFalloff,		0.01f,	8f );


			EditorGUILayout.Space();


			_target.normalScale = showGUISlider( "Normal Scale", _target.normalScale, 0.01f, 1f );


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Randomise Cloud Seed", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise Cloud Seed" );
					_target.seedCloud = Random.Range( 0, int.MaxValue );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();


			showingCloudNoise					= showGUINoise( _target.cloudNoise, "Cloud Noise", showingCloudNoise );


			EditorGUILayout.Space();


			_target.cloudWorleyNoiseOctaves		= showGUISlider( "Cloud Worley Octaves",	_target.cloudWorleyNoiseOctaves,	0,		10 );
			_target.cloudWorleyNoiseFrequency	= showGUISlider( "Cloud Worley Frequency",	_target.cloudWorleyNoiseFrequency,	0.1f,	100f );
			_target.cloudWorleyNoiseAmplitude	= showGUISlider( "Cloud Worley Amplitude",	_target.cloudWorleyNoiseAmplitude,	0.1f,	10f );


			EditorGUILayout.Space();


			_target.cloudShadowRange			= showGUISlider( "Cloud Shadow Range",		_target.cloudShadowRange,			0f,		100f );
			_target.cloudShadowStrength			= showGUISlider( "Cloud Shadow Strength",	_target.cloudShadowStrength,		0f,		1f );


			EditorGUILayout.Space();


			_target.cloudColour0				= showColourPicker( "Cloud Colour 0",		_target.cloudColour0 );
			_target.cloudColour1				= showColourPicker( "Cloud Colour 1",		_target.cloudColour1 );


			EditorGUILayout.Space();


			_target.cloudSpin					= showGUISlider( "Cloud Spin",				_target.cloudSpin,					1f,		10f );
			_target.cloudNormalScale			= showGUISlider( "Cloud Normal Scale",		_target.cloudNormalScale,			0.01f,	1f );
			
			
			EditorGUILayout.Space();


			int size = EditorGUILayout.Popup( "Size", _target.size, sizeOptions );

			if( size != _target.size )
			{
				Undo.RecordObject( _target, "Size" );
				_target.size = size;
			}


			bool combineHeightWithCity = EditorGUILayout.Toggle( "Bake Height into Illumination", _target.combineHeightWithCity );

			if( combineHeightWithCity != _target.combineHeightWithCity )
			{
				Undo.RecordObject( _target, "Bake Height into Illumination" );
				_target.combineHeightWithCity = combineHeightWithCity;
			}


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Preview", EditorStyles.miniButton ) )
				{
					EditorApplication.delayCall += PlanetPreviewWindow.ShowWindow;
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
				
				if( GUILayout.Button( "Full Bake", EditorStyles.miniButton ) )
				{
					EditorApplication.delayCall += textureGenerator.bakeGroundTextures;
					EditorApplication.delayCall += textureGenerator.bakeCloudTextures;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Bake Ground Maps", EditorStyles.miniButton ) )
					EditorApplication.delayCall += textureGenerator.bakeGroundTextures;
				
				if( GUILayout.Button( "Bake Cloud Maps", EditorStyles.miniButton ) )
					EditorApplication.delayCall += textureGenerator.bakeCloudTextures;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		private bool showGUINoise( PlanetNoise planetNoise, string name, bool show )
		{
			EditorGUILayout.BeginHorizontal();
			{
				show = EditorGUILayout.Foldout( show, name );

				if( GUILayout.Button( "Randomise", EditorStyles.miniButton ) )
				{
					Undo.RecordObject( _target, "Randomise " + name );
					_target.randomiseNoise( planetNoise );
					EditorApplication.delayCall += updatePreviewIfAvailable;
				}
			}
			EditorGUILayout.EndHorizontal();

			++EditorGUI.indentLevel;
			
			if( show )
			{
				planetNoise.scale			= showGUISlider( "scale",			planetNoise.scale,			0.1f,	20f );
				planetNoise.octaves			= showGUISlider( "octaves",			planetNoise.octaves,		0,		10 );
				planetNoise.falloff			= showGUISlider( "falloff",			planetNoise.falloff,		0f,		20f );
				planetNoise.intensity		= showGUISlider( "intensity",		planetNoise.intensity,		0f,		20f );
				planetNoise.ridginess		= showGUISlider( "rigidness",		planetNoise.ridginess,		0f,		1f );
				planetNoise.smearScale		= showGUISlider( "smearScale",		planetNoise.smearScale,		0.1f,	20f );
				planetNoise.smearOctaves	= showGUISlider( "smearOctaves",	planetNoise.smearOctaves,	0,		10 );
				planetNoise.smearFalloff	= showGUISlider( "smearFalloff",	planetNoise.smearFalloff,	0f,		20f );
				planetNoise.smearIntensity	= showGUISlider( "smearIntensity",	planetNoise.smearIntensity,	0f,		20f );
			}
			
			--EditorGUI.indentLevel;
			
			return show;
		}
		
		private float showGUISlider( string name, float value, float min, float max )
		{
			int oldIndent = EditorGUI.indentLevel;
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel( name );
				EditorGUI.indentLevel	= 0;
				float v					= EditorGUILayout.Slider( value, min, max );
				EditorGUI.indentLevel	= oldIndent;

				if( v != value )
				{
					Undo.RecordObject( _target, name );
					value = v;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			return value;
		}
		
		private int showGUISlider( string name, int value, int min, int max )
		{
			int oldIndent = EditorGUI.indentLevel;
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel( name );
				EditorGUI.indentLevel	= 0;
				int v					= EditorGUILayout.IntSlider( value, min, max );
				EditorGUI.indentLevel	= oldIndent;

				if( v != value )
				{
					Undo.RecordObject( _target, name );
					value = v;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			return value;
		}
		
		private Color showColourPicker( string name, Color colour )
		{
			int oldIndent = EditorGUI.indentLevel;
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel( name );
				EditorGUI.indentLevel	= 0;
				Color c					= EditorGUILayout.ColorField( colour );
				EditorGUI.indentLevel	= oldIndent;

				if( c != colour )
				{
					Undo.RecordObject( _target, name );
					colour = c;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			return colour;
		}
	}
}