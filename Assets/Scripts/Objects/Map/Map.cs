using System;
using System.Drawing;
using System.Collections.Generic;
using UnityEngine;

namespace Libraries.Map
{
	// TODO: SECTOR_SIZE to struct
	// TODO: replace sector location by proportions

	public enum TILE_TYPE
	{
		DIRT,
		SNOW,
		SAND,
		STONE,
		ASPHALT,
		WATER,
		LAVA,
		ACID,
		DESTROYED
	}
	public enum SECTOR_TYPE
	{
		PLAIN,
		ROAD,
		HILL,
		MOUNTAIN,
		PIT,
		RIVER,
		LAKE,
		SEA
	}
	public enum SECTOR_SHAPE
	{
		TRIANGLE,
		SQUARE,
		RECTANGLE,
		ELLIPSE
	}
	public enum SECTOR_SIZE
	{
		TINY = 9,
		SMALL = 25,
		MEDIUM = 49,
		BIG = 81,
		HUGE = 144
	}
	public enum MAP_TYPE
	{
		PLAINS,
		DESERT,
		MOUNTAIN_RANGE,
		CITY,
		ISLAND,
		ARCHIPELAGO,
		OCEAN
	}

	public abstract class Tile
	{
		public abstract class TileCreator
		{
			public abstract Tile create(Point location, float height = 0);
		}

		public Point location;
		public float height;
		public sbyte durability;
		public readonly bool is_destructible;

		public Tile(Point location, float height = 0)
		{
			this.location=location;
			this.height=height;
		}

		public virtual void destroy()
		{
			return;
		}
	}
	public class TileTest : Tile
	{
		public class TileDirtCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileDirt(location, height);
			}
		}

		public TileTest(Point location, float height = 0) : base(location, height)
		{
			durability=1;
			this.location=location;
		}

		public override void destroy()
		{
			return;
		}
	}
	public class TileDirt : Tile
	{
		public class TileDirtCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileDirt(location, height);
			}
		}

		public TileDirt(Point location, float height = 0) : base(location, height)
		{
			durability=1;
			this.location=location;
		}

		public override void destroy()
		{
			return;
		}
	}

	public abstract class Sector
	{
		public struct SectorProportions
		{
			public Point center, radius, third_point;

			public SectorProportions(Point center, Point radius, Point third_point)
			{
				this.center = center;
				this.radius = radius;
				this.third_point = third_point;
			}
		}

		public readonly Point location;
		protected SectorProportions _proportions=new SectorProportions();
		public readonly bool is_destructible;
		public bool is_fogged;
		public sbyte owner;     // captured color

		public abstract SECTOR_SIZE Size
		{ get; protected set; }
		public abstract SECTOR_SHAPE Shape
		{ get; protected set; }
		/*protected abstract SectorProportions Proportions
		{ get; set; }*/

		public Sector(Point location, SECTOR_SIZE size, SECTOR_SHAPE shape, bool is_destructible = true)
		{
			this.location = location;
			Shape=shape;
			Size=size;
			this.is_destructible=is_destructible;
		}

		public List<Tile> fillShape(Tile[,] tiles_map, Tile.TileCreator tile_creator)
		{
			switch(Shape)
			{
				case SECTOR_SHAPE.TRIANGLE:
					return default;
				case SECTOR_SHAPE.SQUARE:
					return default;
				case SECTOR_SHAPE.RECTANGLE:
					return default;
				case SECTOR_SHAPE.ELLIPSE:
					_proportions.center.X = location.X+(int)Math.Sqrt((int)Size)/2;
					_proportions.center.Y = location.Y+(int)Math.Sqrt((int)Size)/2;
					_proportions.radius.X=Utility.Random.Next((int)Math.Sqrt((int)Size)/2, (int)Math.Sqrt((int)Size));
					_proportions.radius.Y=Utility.Random.Next((int)Math.Sqrt((int)Size)/2, (int)Math.Sqrt((int)Size));
					return fillEllipse(tiles_map, tile_creator);
				default:
					break;
			}
			return default;
		}
		public virtual List<Tile> fillEllipse(Tile[,] tiles_map, Tile.TileCreator tile_creator)
		{
			List<Tile> tiles_ellipse = new List<Tile>();
			float distance;

			for (int i = location.X; i<location.X+(int)Math.Sqrt((int)Size); i++)
			{
				for (int j = location.Y; j<location.Y+(int)Math.Sqrt((int)Size); j++)
				{
					distance=Vector2.Distance(new Vector2(_proportions.center.X, _proportions.center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
					if(distance<=_proportions.radius.X && distance<=_proportions.radius.Y)
					{
						tiles_map[i, j]=tile_creator.create(tiles_map[i, j].location, Math.Max(_proportions.radius.X, _proportions.radius.Y)/(10*Math.Max(distance, 1.0f)));
						tiles_ellipse.Add(tiles_map[i, j]);
					}
				}
			}

			return tiles_ellipse;
		}
		public abstract float[,] generate(Tile[,] tiles_map);
		public virtual void populate()          //example args: Object building
		{

		}
	}
	public class SectorPlain : Sector
	{
		public override SECTOR_SIZE Size
		{ get; protected set; }
		public override SECTOR_SHAPE Shape
		{ get; protected set; }
		/*protected override SectorProportions Proportions
		{ get; set; }*/

		public SectorPlain(Point location, SECTOR_SHAPE shape, SECTOR_SIZE size=SECTOR_SIZE.MEDIUM) : base(location, size, shape, true)
		{

		}

		public override float[,] generate(Tile[,] tiles_map)
		{
			fillShape(tiles_map, new TileDirt.TileDirtCreator());

			return default;
		}
	}

	public class Map
	{
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
			SECTOR_SIZE sector_size= Utility.getRandomEnum<SECTOR_SIZE>();

			for(int i=0; i<length-(int)Math.Sqrt((int)sector_size); i+=(int)Math.Sqrt((int)sector_size)) //Utility.Random.Next((int)Math.Sqrt((int)SECTOR_SIZE.MEDIUM))
			{
				for(int j=0; j<height-(int)Math.Sqrt((int)sector_size); j+=(int)Math.Sqrt((int)sector_size))
				{
					switch(Utility.getRandomEnum<SECTOR_TYPE>())
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
							sector=new SectorPlain(new Point(i, j), SECTOR_SHAPE.ELLIPSE, sector_size);
							break;
					}
					sector.generate(tiles);
					_sectors.Add(sector);
					sector_size= Utility.getRandomEnum<SECTOR_SIZE>();
				}
			}
		}
	}
}