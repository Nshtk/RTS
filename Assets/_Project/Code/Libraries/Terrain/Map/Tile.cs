using System;
using System.Drawing;

namespace Libraries.Terrain
{
	public abstract class Tile
	{
		public enum TYPE
		{
			VOID=0,
			DESTROYED=1,
			GROUND=2,
			DIRT=3,
			SAND=4,
			SNOW=5,
			STONE=6,
			CONCRETE=7,
			WATER=8,
			LAVA=9,
			ACID=10
		}
		public abstract class TileCreator
		{
			public abstract Tile create(Point location, float height = 0);
		}

		public Point location;
		public float height;
		public float alpha;
		public sbyte durability;
		public readonly bool is_destructible;

		public abstract TYPE Type
		{
			get;
		}


		public Tile(Point location, float height = 0, float alpha=1)
		{
			this.location=location;
			TerrainGenerator.height_map[location.X, location.Y]=this.height=height;
			for(int i = 0; i<TerrainGenerator.alpha_map.GetLength(2); i++)
				TerrainGenerator.alpha_map[location.X, location.Y, i]=0;
			TerrainGenerator.alpha_map[location.X, location.Y, (int)Type]=this.alpha=alpha;
		}
		public Tile()		// Workaround for map init by TileTest
		{}

		public virtual void destroy()
		{
			return;
		}
	}
	public class TileTest : Tile
	{
		public class TileTestCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileTest(location, height);
			}
		}

		public override TYPE Type
		{
			get;
		} = TYPE.VOID;

		public TileTest(Point location, float height = 0) : base(location)
		{
			this.location=location;
			durability=1;
		}

		public override void destroy()
		{
			return;
		}
	}
	public sealed class TileGround : Tile
	{
		public class TileGroundCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileGround(location, height);
			}
		}

		public override TYPE Type
		{
			get;
		} = TYPE.GROUND;

		public TileGround(Point location, float height = 0) : base(location, height)
		{
			durability=1;
		}

		public override void destroy()
		{
			return;
		}
	}
	public sealed class TileSand : Tile
	{
		public class TileSandCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileSand(location, height);
			}
		}

		public override TYPE Type
		{
			get;
		} = TYPE.SAND;

		public TileSand(Point location, float height = 0) : base(location, height)
		{
			durability=1;
		}

		public override void destroy()
		{
			return;
		}
	}
	public sealed class TileSnow : Tile
	{
		public class TileSnowCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileSnow(location, height);
			}
		}

		public override TYPE Type
		{
			get;
		} = TYPE.SNOW;

		public TileSnow(Point location, float height = 0) : base(location, height)
		{
			durability=1;
		}

		public override void destroy()
		{
			return;
		}
	}
	public sealed class TileStone : Tile
	{
		public class TileStoneCreator : TileCreator
		{
			public override Tile create(Point location, float height = 0)
			{
				return new TileStone(location, height);
			}
		}

		public override TYPE Type
		{
			get;
		} = TYPE.STONE;

		public TileStone(Point location, float height = 0) : base(location, height)
		{
			durability=1;
		}

		public override void destroy()
		{
			return;
		}
	}
}
