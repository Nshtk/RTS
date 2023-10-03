using System;
using System.Drawing;
using System.Collections.Generic;
using UnityEngine;

namespace Libraries.Map
{
	public abstract class Tile
	{
		public enum TYPE
		{
			DIRT,
			SNOW,
			SAND,
			STONE,
			CONCRETE,
			WATER,
			LAVA,
			ACID,
			DESTROYED
		}
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
	public sealed class TileDirt : Tile
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
		public enum TYPE
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
		public enum SIZE
		{
			TINY = 6,
			SMALL = 8,
			MEDIUM = 10,
			BIG = 12,
			HUGE = 14
		}
		public enum FORM
		{
			SQUARE,
			RECTANGLE_HORIZONTAL,
			RECTANGLE_VERTICAL
		}
		public sealed class SectorFiller
		{
			public enum SHAPE
			{
				TRIANGLE,
				SQUARE,
				RECTANGLE,
				ELLIPSE
			}
			public SHAPE shape;
			public Point center, radius, third_point;
			public SectorFiller(SHAPE shape, Point center, Point radius, Point third_point)
			{
				this.shape = shape;
				this.center = center;
				this.radius = radius;
				this.third_point = third_point;
			}
		}

		public readonly Point location;
		public readonly SIZE size;
		public readonly Vector2 proportions;
		public readonly bool is_destructible;		// to interface IDestructible?
		public bool is_fogged;
		public sbyte owner;     // captured color

		public abstract FORM Form
		{ get; protected set; }
		public abstract SectorFiller Sector_Filler
		{ get; protected set; }

		public Sector(Point location, FORM form, SIZE size, SectorFiller.SHAPE shape, bool is_destructible = true)
		{
			this.location = location;
			this.size = size;
			Form = form;
			switch(form)
			{
				case FORM.SQUARE:
					proportions=new Vector2(1, 1);
					break;
				case FORM.RECTANGLE_HORIZONTAL:
					proportions=new Vector2(Utility.Random.Next(2, 6), 1);
					break;
				case FORM.RECTANGLE_VERTICAL:
					proportions=new Vector2(1, Utility.Random.Next(2, 6));
					break;
				default:
					break;
			}
			proportions.x*=(int)this.size;
			proportions.y*=(int)this.size;

			Sector_Filler=new SectorFiller(shape, default, default, default);
			this.is_destructible=is_destructible;
		}

		public List<Tile> fillShape(Tile[,] tiles_map, Tile.TileCreator tile_creator)
		{
			switch(Sector_Filler.shape)
			{
				case SectorFiller.SHAPE.TRIANGLE:
					return default;
				case SectorFiller.SHAPE.SQUARE:
					return default;
				case SectorFiller.SHAPE.RECTANGLE:
					return default;
				case SectorFiller.SHAPE.ELLIPSE:
					Sector_Filler.center.X = location.X+(int)proportions.x/2;
					Sector_Filler.center.Y = location.Y+(int)proportions.y/2;
					Sector_Filler.radius.X=Utility.Random.Next((int)proportions.x/2, (int)proportions.x);
					Sector_Filler.radius.Y=Utility.Random.Next((int)proportions.y/2, (int)proportions.y);
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

			for(int i=location.X; i<location.X+(int)proportions.x; i++)
			{
				for(int j=location.Y; j<location.Y+(int)proportions.y; j++)
				{
					distance=Vector2.Distance(new Vector2(Sector_Filler.center.X, Sector_Filler.center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
					if(distance<=Sector_Filler.radius.X && distance<=Sector_Filler.radius.Y)
					{
						tiles_map[i, j]=tile_creator.create(tiles_map[i, j].location, Math.Max(Sector_Filler.radius.X, Sector_Filler.radius.Y)/(10*Math.Max(distance, 1.0f)));
						tiles_ellipse.Add(tiles_map[i, j]);
					}
				}
			}

			return tiles_ellipse;
		}
		public abstract float[,] generate(Tile[,] tiles_map);
		public virtual void populate()          //example args: GameObject building
		{

		}
	}
	public sealed class SectorPlain : Sector
	{
		public override FORM Form
		{ get; protected set; }
		public override SectorFiller Sector_Filler
		{ get; protected set; }
		public SectorPlain(Point location, FORM form=FORM.SQUARE, SIZE size=SIZE.MEDIUM, SectorFiller.SHAPE shape=SectorFiller.SHAPE.SQUARE) : base(location, form, size, shape)
		{

		}

		public override float[,] generate(Tile[,] tiles_map)
		{
			fillShape(tiles_map, new TileDirt.TileDirtCreator());

			return default;
		}
	}

	public sealed class Map
	{
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
							sector=new SectorPlain(new Point(j, i), Sector.FORM.SQUARE, Utility.getRandomEnum<Sector.SIZE>(), Sector.SectorFiller.SHAPE.ELLIPSE);
							break;
					}
					j+=(int)sector.proportions.x-1;
					if(j>length)
						break;
					/*if((i+(int)sector_proportions.y)>height)
					{
						i=height;
						break;
					}*/
					sector.generate(tiles);
					_sectors.Add(sector);
					if(sector_proportions_max_y.y<sector.proportions.y)
						sector_proportions_max_y=sector.proportions;
				}
				i+=(int)sector_proportions_max_y.y-1;
			}
		}
	}
}