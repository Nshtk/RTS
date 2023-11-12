using System.Drawing;
using System.Collections.Generic;
using UnityEngine;

namespace Libraries.Terrain
{
	public sealed class Map
	{
		public enum MAP_TYPE
		{
			PLAIN,
			DESERT,
			MOUNTAIN_RANGE,
			CITY,
			ISLAND,
			ARCHIPELAGO,
			OCEAN
		}
		public int length, height;
		public readonly Tile[,] tiles;
		private readonly List<Sector> _sectors = new List<Sector>();

		public Map(int length, int height)
		{
			this.length = length;
			this.height = height;
			tiles=new Tile[length, height];
			for(int i=0; i<length; i++)
				for(int j=0; j<height; j++)
					tiles[i, j]=new TileTest(new Point(i, j), 0);
		}

		public void generateRandom()
		{
			Sector sector;
			Vector2 sector_proportions_max_y=new Vector2(0, 0);

			for(int i=0; i<height;)
			{
				for(int j=0; j<length;)
				{
					switch(Utility.getRandomEnum<Sector.TYPE>())
					{
						/*case SECTOR_TYPE.ROAD:
							break;
						case SECTOR_TYPE.HILL:
							break;
						case SECTOR_TYPE.MOUNTAIN:
							break;
						case SECTOR_TYPE.PIT:
							break;
						case SECTOR_TYPE.RIVER:
							break;
						case SECTOR_TYPE.LAKE:
							break;
						case SECTOR_TYPE.SEA:
							break;*/
						default:
							sector=new SectorMountain(new Point(j, i), Sector.FORM.RECTANGLE_HORIZONTAL, Utility.getRandomEnum<Sector.SIZE>(), Sector.SectorFiller.SHAPE.ELLIPSE);
							break;
					}
					j+=(int)sector.proportions.x;
					if(sector_proportions_max_y.y<sector.proportions.y)
						sector_proportions_max_y=sector.proportions;
					if(j>length || i+(int)sector.proportions.y>height)
						break;

					sector.generate(tiles);
					_sectors.Add(sector);
				}
				i+=(int)sector_proportions_max_y.y;
			}
		}
		public void generatePlain()		// + more methods according to map's enum
		{
			
		}
	}
}