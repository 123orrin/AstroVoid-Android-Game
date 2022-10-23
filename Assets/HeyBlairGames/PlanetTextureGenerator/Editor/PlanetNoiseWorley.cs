using UnityEngine;

namespace HeyBlairGames.PlanetTextureGenerator.Editor
{
	public class PlanetNoiseWorley
	{
		public int seed = 0;

		public PlanetNoiseWorley( int seed )
		{
			this.seed = seed;
		}

		public float getOctave( Vector3 pos, int octaves, float amplitude )
		{
			float result	= 0f;
			float scale		= 1f;
		
			for( int i = 0; i < octaves; ++i )
			{
				result	+= noise( pos * scale ) * ( amplitude / scale );
				scale	*= 2f;
			}

			return result;
		}

		
		private float noise( Vector3 pos )
		{
			float result	= float.MaxValue;
			
			int cellX		= Mathf.FloorToInt( pos.x );
			int cellY		= Mathf.FloorToInt( pos.y );
			int cellZ		= Mathf.FloorToInt( pos.z );

			float diffDiv	= 1f / 0xffffffff;

			for( int i = cellX - 1; i <= cellX + 1; ++i )
			{
				for( int j = cellY - 1; j <= cellY + 1; ++j )
				{
					for( int k = cellZ - 1; k <= cellZ + 1; ++k )
					{
						uint hash		= hashFNV( ( uint ) ( i + seed ), ( uint ) j, ( uint ) k );
						uint random		= randomLCG( hash );
						
						uint pointCount	= getPointProbabilityCount( random );
						
						for( uint a = 0; a < pointCount; ++a )
						{
							random			= randomLCG( random );
							float diffX		= random * diffDiv;

							random			= randomLCG( random );
							float diffY		= random * diffDiv;

							random			= randomLCG( random );
							float diffZ		= random * diffDiv;

							Vector3 point	= new Vector3( diffX + i, diffY + j, diffZ + k );
							result			= Mathf.Min( result, Vector3.SqrMagnitude( pos - point ) );
						}
					}
				}
			}

			result = Mathf.Clamp01( result );
			return result;
		}
		
		//Poisson distribution - mean density = 4, max points = 9
		private uint getPointProbabilityCount( uint value )
		{
			if( value < 393325350 )		return 1;
			if( value < 1022645910 )	return 2;
			if( value < 1861739990 )	return 3;
			if( value < 2700834071 )	return 4;
			if( value < 3372109335 )	return 5;
			if( value < 3819626178 )	return 6;
			if( value < 4075350088 )	return 7;
			if( value < 4203212043 )	return 8;
			return 9;
		}
	
		//linear congruential generator - c values
		private uint randomLCG( uint lastValue )
		{
			uint result	= 1103515245u * lastValue + 12345u;
			return result;
		}
		
		private uint hashFNV( uint i, uint j, uint k )
		{
			uint OFFSET_BASIS	= 2166136261;
			uint FNV_PRIME		= 16777619;

			uint result			= OFFSET_BASIS ^ i;
			result				*= FNV_PRIME;
			result				^= j;
			result				*= FNV_PRIME;
			result				^= k;
			result				*= FNV_PRIME;

			return result;
		}
	}
}