using UnityEditor;
using UnityEngine;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetPreviewWindow : EditorWindow
	{
		public const int	labelGap	= 20;
		public const int	rowGap		= 25;

		static public void ShowWindow()
		{
			EditorWindow.GetWindow< PlanetPreviewWindow >();
		}

		public void updatePreview()
		{
			if( Selection.activeObject != null )
			{
				if( Selection.activeObject is PlanetAsset && Selection.activeObject != _target )
					_target = Selection.activeObject as PlanetAsset;
			}

			if( textureGenerator == null )
				textureGenerator = new PlanetTextureGenerator();

			textureGenerator._target = _target;
			textureGenerator.updatePreview();
		}

		public void OnGUI()				{ if( textureGenerator != null ) showPreviewGUI(); }
		public void OnSelectionChange()	{ updateTargetIfChanged(); }
		public void OnInspectorUpdate()	{ Repaint(); }
	
	
		private PlanetAsset				_target;
		private PlanetTextureGenerator	textureGenerator;

		private Vector2					scrollPosition;

		private void showPreviewGUI()
		{
			GUILayout.Space( 5 );

			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUILayout.MaxWidth( 537 ), GUILayout.MaxHeight( 470 ) );

			Rect paramRect = EditorGUILayout.BeginVertical( GUILayout.Width( 522 ), GUILayout.Height( 455 ) );
			{
				//row 0
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Diffuse", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.diffusePreview );
					}
					EditorGUILayout.EndVertical();

					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Specular", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawTextureAlpha( paramRect, textureGenerator.diffusePreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();


				GUILayout.Space( rowGap );

				//row 1
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Normal", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.normalPreview );
					}
					EditorGUILayout.EndVertical();
				
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Height", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawTextureAlpha( paramRect, textureGenerator.normalPreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();


				GUILayout.Space( rowGap );
			
				//row 2
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Illumination", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.illuminationPreview );
					}
					EditorGUILayout.EndVertical();
				
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Cloud", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.cloudPreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndScrollView();
		}

		private void updateTargetIfChanged()
		{
			if( Selection.activeObject != null )
			{
				if( Selection.activeObject is PlanetAsset && Selection.activeObject != _target )
					updatePreview();
			}
		}
	}
}