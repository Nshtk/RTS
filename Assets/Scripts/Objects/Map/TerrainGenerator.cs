using UnityEngine;
using System.Drawing;
using System.Collections.Generic;
using Libraries;
using System;

namespace RTS.Map
{
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
		TINY=9,
		SMALL=25,
		MEDIUM=49,
		BIG=81,
		HUGE=144
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

	public class Map
	{
		public int length, width;
		private readonly Tile[,] _tiles;
		private readonly List<Sector> _sectors=new List<Sector>();

		public Map(int length, int width)
		{
			_tiles=new Tile[length, width];
		}

		public void generateRandom()
		{
			Sector sector;
			Sector.SectorShape[] shapes = new Sector.SectorShape[] { new Sector.SectorEllipse() };

			for(int i=0; i<length; i++)
			{
				for(int j=0; j<width; j++)
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
							sector=new SectorPlain(shapes[Utility.Random.Next(shapes.Length)], SECTOR_SIZE.MEDIUM);
							break;
					}
					_sectors.Add(sector);
				}
			}
		}
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

		public Tile(Point location, float height=0)
		{
			this.location=location;
			this.height=height;
		}

		public virtual void destroy()
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

		public TileDirt(Point location, float height=0) : base(location, height)
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
		public abstract class SectorShape
		{
			public virtual List<Tile> fill()
			{
				return default;
			}
			public virtual List<Tile> fill(Tile[,] tiles_map, Tile.TileCreator tile_creator, Point center, Point radius)
			{
				return default;
			}
		}
		public class SectorEllipse : SectorShape
		{
			public override List<Tile> fill(Tile[,] tiles_map, Tile.TileCreator tile_creator, Point center, Point radius)
			{
				List<Tile> tiles_ellipse = new List<Tile>();
				float distance;

				for (int i = 0; i<tiles_map.GetLength(0); i++)
				{
					for (int j = 0; j<tiles_map.GetLength(1); j++)
					{
						distance=Vector2.Distance(new Vector2(center.X, center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
						if (distance<=radius.X && distance<=radius.Y)
						{
							tiles_map[i, j]=tile_creator.create(tiles_map[i, j].location, tiles_map[i, j].height);
							tiles_ellipse.Add(tiles_map[i, j]);
						}
					}
				}

				return tiles_ellipse;
			}
		}

		protected SectorShape shape;
		public readonly bool is_destructible;
		public bool is_fogged;
		public sbyte owner;		// captured color
		public abstract SECTOR_SIZE Size
		{ get; protected set; }
		public abstract Vector2 Proportions
		{ get; protected set; }

		public Sector(SectorShape shape, SECTOR_SIZE size, Vector2 proportions, bool is_destructible=true)
		{
			this.shape=shape;
			Size=size;
			Proportions=proportions;
			this.is_destructible=is_destructible;
		}

		
		public abstract float[,] generate(Tile[,] tiles_map);
		public virtual void populate()			//example args: Object building
		{

		}
	}
	public class SectorPlain : Sector
	{
		public override SECTOR_SIZE Size
		{ get; protected set; }
		public override Vector2 Proportions
		{ get; protected set; }


		public SectorPlain(SectorShape shape, SECTOR_SIZE size=SECTOR_SIZE.MEDIUM, Vector2 proportions=default) : base(shape, size, proportions, true)
		{
			
		}

		public override float[,] generate(Tile[,] tiles_map) 
		{
			Point center = new Point(), radius=new Point();

			center.X = (int)Math.Sqrt((int)Size)/2;
			center.Y = center.X;
			radius.X=(center.X-1)/Utility.Random.Next(3);
			radius.Y=(center.X-1)/Utility.Random.Next(3);
			shape.fill(tiles_map, new TileDirt.TileDirtCreator(), center, radius);

			return default;
		}
	}
	/*public class SectorHill : Sector
	{


		public override Vector2 Proportions
		{ get; protected set; }
		public override int Shape
		{ get; protected set; }

		public SectorHill(int shape, Vector2 Proportions) : base(shape, Proportions, true)
		{

		}

		public override float[,] generate()
		{
			return default;
		}
	}*/


	public class TerrainGenerator : MonoBehaviour
	{
		private Terrain _terrain;

		public int length = 256;
		public int width = 256;
		public int height = 20;
		public float scale = 20f;
		public float f;

		private void Start()
		{
			_terrain = GetComponent<Terrain>();
			_terrain.terrainData.heightmapResolution = width+1;
			_terrain.terrainData.size=new Vector3(length, height, width);
			_terrain.terrainData.SetHeights(0, 0, generateHeightMap());
		}
		private void Update()
		{

		}

		private float[,] generateHeightMap()
		{
			float[,] height_map = new float[length, width];

			for (int i=0; i<length; i++)
				for (int j=0; j<width; j++)
					height_map[i, j]=Mathf.PerlinNoise((float)i/length*scale, (float)j/width*scale);

			return height_map;
		}
	}
}