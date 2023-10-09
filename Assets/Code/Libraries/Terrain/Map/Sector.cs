using System.Collections.Generic;
using System.Drawing;
using System;
using UnityEngine;

namespace Libraries.Terrain
{
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
			public Point radius, center, third_point;
			public SectorFiller(SHAPE shape, Point radius, Point center, Point third_point)
			{
				this.shape = shape;
				this.radius = radius;
				this.center = center;
				this.third_point = third_point;
			}
		}

		public readonly Point location;
		public readonly SIZE size;
		public readonly Vector2 proportions;
		public readonly bool is_destructible;       // to interface IDestructible?
		public bool is_fogged;
		public sbyte owner;     // captured color

		public abstract FORM Form
		{
			get; protected set;
		}
		public abstract SectorFiller Sector_Filler
		{
			get; protected set;
		}

		public Sector(Point location, FORM form, SIZE size, SectorFiller.SHAPE filler_shape, bool is_destructible = true)
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

			Sector_Filler=new SectorFiller(filler_shape, default, default, default);
			this.is_destructible=is_destructible;
		}

		protected List<Tile> fillShape(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			switch(Sector_Filler.shape)
			{
				case SectorFiller.SHAPE.TRIANGLE:
					return fillTriangle(tiles_map, tile_creators);
				case SectorFiller.SHAPE.SQUARE:
					Sector_Filler.radius.X=Utility.Random.Next(1, (int)size/5);
					Sector_Filler.radius.Y=Sector_Filler.radius.X;
					return fillSquare(tiles_map, tile_creators);
				case SectorFiller.SHAPE.RECTANGLE:
					return fillRectangle(tiles_map, tile_creators);
				case SectorFiller.SHAPE.ELLIPSE:
					Sector_Filler.radius.X=Utility.Random.Next((int)proportions.x/2, (int)proportions.x);
					Sector_Filler.radius.Y=Utility.Random.Next((int)proportions.y/2, (int)proportions.y);
					Sector_Filler.center.X = location.X+(int)proportions.x/2;
					Sector_Filler.center.Y = location.Y+(int)proportions.y/2;
					return fillEllipse(tiles_map, tile_creators);
				default:
					break;
			}
			return default;
		}
		protected virtual List<Tile> fillTriangle(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			return default;
		}
		protected virtual List<Tile> fillSquare(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			List<Tile> tiles_ellipse = new List<Tile>();

			for(int i = location.X+Sector_Filler.radius.X; i<location.X+(int)proportions.x-Sector_Filler.radius.X; i++)
			{
				for(int j = location.Y+Sector_Filler.radius.Y; j<location.Y+(int)proportions.y-Sector_Filler.radius.Y; j++)
				{
					tiles_ellipse.Add(tiles_map[i, j]=tile_creators[0].create(tiles_map[i, j].location));       // TODO: body to overridable function 
					TerrainGenerator.height_map[i, j]=tiles_map[i, j].height;
				}
			}

			return tiles_ellipse;
		}
		protected virtual List<Tile> fillRectangle(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			return default;
		}
		protected virtual List<Tile> fillEllipse(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			List<Tile> tiles_ellipse = new List<Tile>();
			float distance;

			for(int i = location.X; i<location.X+(int)proportions.x; i++)
			{
				for(int j = location.Y; j<location.Y+(int)proportions.y; j++)
				{
					distance=Vector2.Distance(new Vector2(Sector_Filler.center.X, Sector_Filler.center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
					if(distance<=Sector_Filler.radius.X && distance<=Sector_Filler.radius.Y)
						tiles_ellipse.Add(tiles_map[i, j]=tile_creators[0].create(tiles_map[i, j].location));
					//TerrainGenerator.alpha_map[i, j, (int)tiles_map[i, j].Type]=0;
					TerrainGenerator.height_map[i, j]=tiles_map[i, j].height;
				}
			}

			return tiles_ellipse;
		}
		public abstract float[,] generate(Tile[,] tiles_map);
		public virtual void populate()          //example args: GameObject building
		{

		}
	}
	public class SectorPlain : Sector
	{
		public override FORM Form       // properties to validate values 
		{
			get; protected set;
		}
		public override SectorFiller Sector_Filler
		{
			get; protected set;
		}
		public SectorPlain(Point location, FORM form = FORM.SQUARE, SIZE size = SIZE.MEDIUM, SectorFiller.SHAPE filler_shape = SectorFiller.SHAPE.SQUARE) : base(location, form, size, filler_shape)
		{

		}

		public override float[,] generate(Tile[,] tiles_map)
		{
			fillShape(tiles_map, new Tile.TileCreator[] { new TileGround.TileGroundCreator() });

			return default;
		}
	}
	public class SectorHill : Sector
	{
		public override FORM Form
		{
			get; protected set;
		}
		public override SectorFiller Sector_Filler
		{
			get; protected set;
		}
		public SectorHill(Point location, FORM form = FORM.SQUARE, SIZE size = SIZE.MEDIUM, SectorFiller.SHAPE filler_shape = SectorFiller.SHAPE.ELLIPSE) : base(location, form, size, filler_shape)
		{

		}

		protected override List<Tile> fillEllipse(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			List<Tile> tiles_ellipse = new List<Tile>();
			float distance;

			for(int i = location.X; i<location.X+(int)proportions.x; i++)
			{
				for(int j = location.Y; j<location.Y+(int)proportions.y; j++)
				{
					distance=Vector2.Distance(new Vector2(Sector_Filler.center.X, Sector_Filler.center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
					if(distance<=Sector_Filler.radius.X && distance<=Sector_Filler.radius.Y)
					{
						tiles_map[i, j]=tile_creators[0].create(tiles_map[i, j].location, (float)(1/(Math.Log(distance+2)-0.3))); //Math.Max(Sector_Filler.radius.X, Sector_Filler.radius.Y)/(10*Math.Max(distance, 1.0f))
						tiles_ellipse.Add(tiles_map[i, j]);
						//TerrainGenerator.alpha_map[i, j, (int)tiles_map[i, j].Type]=0;
						TerrainGenerator.height_map[i, j]=tiles_map[i, j].height;
					}
				}
			}

			return tiles_ellipse;
		}
		public override float[,] generate(Tile[,] tiles_map)
		{
			fillShape(tiles_map, new Tile.TileCreator[] { new TileGround.TileGroundCreator() });

			return default;
		}
	}
	public class SectorMountain : Sector
	{
		public override FORM Form
		{
			get; protected set;
		}
		public override SectorFiller Sector_Filler
		{
			get; protected set;
		}
		public SectorMountain(Point location, FORM form = FORM.SQUARE, SIZE size = SIZE.MEDIUM, SectorFiller.SHAPE filler_shape = SectorFiller.SHAPE.ELLIPSE) : base(location, form, size, filler_shape)
		{

		}

		protected override List<Tile> fillEllipse(Tile[,] tiles_map, Tile.TileCreator[] tile_creators)
		{
			List<Tile> tiles_ellipse = new List<Tile>();
			float distance, height;

			for(int i = location.X; i<location.X+(int)proportions.x; i++)
			{
				for(int j = location.Y; j<location.Y+(int)proportions.y; j++)
				{
					distance=Vector2.Distance(new Vector2(Sector_Filler.center.X, Sector_Filler.center.Y), new Vector2(tiles_map[i, j].location.X, tiles_map[i, j].location.Y));
					if(distance<=Sector_Filler.radius.X && distance<=Sector_Filler.radius.Y)
					{
						height=(float)(1/(Math.Log(distance+2)-0.3));
						if(height<0.85)
							tiles_map[i, j]=tile_creators[0].create(tiles_map[i, j].location, height); //Math.Max(Sector_Filler.radius.X, Sector_Filler.radius.Y)/(10*Math.Max(distance, 1.0f))
						else
							tiles_map[i, j]=tile_creators[1].create(tiles_map[i, j].location, height);
						tiles_ellipse.Add(tiles_map[i, j]);
					}
				}
			}

			return tiles_ellipse;
		}
		public override float[,] generate(Tile[,] tiles_map)
		{
			fillShape(tiles_map, new Tile.TileCreator[] { new TileStone.TileStoneCreator(), new TileSnow.TileSnowCreator() });

			return default;
		}
	}
}
