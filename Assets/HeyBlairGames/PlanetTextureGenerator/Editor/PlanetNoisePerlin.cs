using UnityEngine;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetNoisePerlin
	{
		public const int gradStride	= 12 * 3;
		public const int permStride	= 512;

		public PlanetNoisePerlin( int seed )
		{
			System.Random rnd = new System.Random( seed );

			grad		= new Vector3[ 12 ];
			grad[  0 ]	= new Vector3(  1,  1,  0 );
			grad[  1 ]	= new Vector3( -1,  1,  0 );
			grad[  2 ]	= new Vector3(  1, -1,  0 );
			grad[  3 ]	= new Vector3( -1, -1,  0 );
			grad[  4 ]	= new Vector3(  1,  0,  1 );
			grad[  5 ]	= new Vector3( -1,  0,  1 );
			grad[  6 ]	= new Vector3(  1,  0, -1 );
			grad[  7 ]	= new Vector3( -1,  0, -1 );
			grad[  8 ]	= new Vector3(  0,  1,  1 );
			grad[  9 ]	= new Vector3(  0, -1,  1 );
			grad[ 10 ]	= new Vector3(  0,  1, -1 );
			grad[ 11 ]	= new Vector3(  0, -1, -1 );

			perm = new uint[ permStride ];

			for( int i = 0; i < permStride / 2; ++i )
				perm[ i ] = ( uint ) Mathf.FloorToInt( ( float ) rnd.NextDouble() * ( permStride / 2 ) );

			for( int i = permStride / 2; i < permStride; ++i )
				perm[ i ] = perm[ i & ( permStride / 2 - 1 ) ];
		}

		public float getOctave( Vector3 pos, int octaves )
		{
			float result	= 0f;
			float scale		= 1f;

			for( int i = 0; i < octaves; ++i )
			{
				result	+= noise( pos * scale ) / scale;
				scale	*= 2f;
			}

			return result;
		}
		
		public float getNormalisedOctave( Vector3 pos, int octaves )
		{
			float result	= 0f;
			float scale		= 1f;

			for( int i = 0; i < octaves; ++i )
			{
				result	+= noise( pos * scale ) / scale;
				scale	*= 2f;
			}

			float l	= 2f - ( 1f / Mathf.Pow( 2f, Mathf.Max( octaves - 1f, 0f ) ) );
			result	/= l;
			return result;
		}

		public float noise( Vector3 pos )
		{
			float result = 0.5f * _noise( pos.x, pos.y, pos.z ) + 0.5f;
			return result;
		}

		public float[] getGradArray()
		{
			float[] result = new float[ gradStride ];

			for( int i = 0; i < grad.Length; ++i )
			{
				result[ ( i * 3 ) ]		= grad[ i ].x;
				result[ ( i * 3 ) + 1 ]	= grad[ i ].y;
				result[ ( i * 3 ) + 2 ]	= grad[ i ].z;
			}

			return result;
		}

		public uint[] getPermArray()
		{
			return perm;
		}


		private Vector3[]	grad;
		private uint[]		perm;

		private float _noise( float x, float y, float z )
		{
			int X			= Mathf.FloorToInt( x );
			int Y			= Mathf.FloorToInt( y );
			int Z			= Mathf.FloorToInt( z );
			x				= x - X;
			y				= y - Y;
			z				= z - Z;
			X				= X & 255;
			Y				= Y & 255;
			Z				= Z & 255;

			uint gi000		= perm[ X + perm[ Y + perm[ Z ] ] ] % 12;
			uint gi001		= perm[ X + perm[ Y + perm[ Z + 1 ] ] ] % 12;
			uint gi010		= perm[ X + perm[ Y + 1 + perm[ Z ] ] ] % 12;
			uint gi011		= perm[ X + perm[ Y + 1 + perm[ Z + 1 ] ] ] % 12;
			uint gi100		= perm[ X + 1 + perm[ Y + perm[ Z ] ] ] % 12;
			uint gi101		= perm[ X + 1 + perm[ Y + perm[ Z + 1 ] ] ] % 12;
			uint gi110		= perm[ X + 1 + perm[ Y + 1 + perm[ Z ] ] ] % 12;
			uint gi111		= perm[ X + 1 + perm[ Y + 1 + perm[ Z + 1 ] ] ] % 12;

			float n000		= Vector3.Dot( grad[ gi000 ], new Vector3( x, y, z ) );
			float n100		= Vector3.Dot( grad[ gi100 ], new Vector3( x - 1, y, z ) );
			float n010		= Vector3.Dot( grad[ gi010 ], new Vector3( x, y - 1, z ) );
			float n110		= Vector3.Dot( grad[ gi110 ], new Vector3( x - 1, y - 1, z ) );
			float n001		= Vector3.Dot( grad[ gi001 ], new Vector3( x, y, z - 1 ) );
			float n101		= Vector3.Dot( grad[ gi101 ], new Vector3( x - 1, y, z - 1 ) );
			float n011		= Vector3.Dot( grad[ gi011 ], new Vector3( x, y - 1, z - 1 ) );
			float n111		= Vector3.Dot( grad[ gi111 ], new Vector3( x - 1, y - 1, z - 1 ) );

			float u			= fade( x );
			float v			= fade( y );
			float w			= fade( z );

			float nx00		= Mathf.Lerp( n000, n100, u );
			float nx01		= Mathf.Lerp( n001, n101, u );
			float nx10		= Mathf.Lerp( n010, n110, u );
			float nx11		= Mathf.Lerp( n011, n111, u );
			float nxy0		= Mathf.Lerp( nx00, nx10, v );
			float nxy1		= Mathf.Lerp( nx01, nx11, v );
			float result	= Mathf.Lerp( nxy0, nxy1, w );

			return result;
		}

		private float fade( float t)
		{
			float result = t * t * t * ( t * ( t * 6f - 15f ) + 10f );
			return result;
		}
	}
}