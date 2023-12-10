using UnityEngine;
using System;
using System.Collections.Generic;
using Libraries.Terrain;


public class Game : MonoBehaviour	//Class containing main loop.
{
	public class GameData          //Class for statistics
	{                              //TODO event aggregator (e.g. flag captured -> update team's captured flags)  
		public static GameData instance;
		private Game _game;

		public ulong ticks = 0;		// FIXME: to BigInt
		public int count_units_died;

		public GameData(Game game)
		{
			instance=this;
			_game=game;
		}

		public void handleUnitDied(Unit sender, Unit.UnitDiedEventArgs e)
		{
			count_units_died++;
		}

		public event FlagCapturedEventHandler flagCaptured;
		public delegate void FlagCapturedEventHandler(Flag sender, FlagCapturedEventArgs e);
		public class FlagCapturedEventArgs : EventArgs
		{
			public string Message
			{
				get;
				set;
			}
			public FlagCapturedEventArgs(string message)
			{
				Message = message;
			}
		}
	}

	[SerializeField] private Human _prefab_human;
	[SerializeField] private Spawn _prefab_spawn;	
	[SerializeField] private Bot _prefab_bot;
	[SerializeField] private TerrainGenerator _prefab_terrain_generator;

	public static Game instance;
	public GameData game_data;
	private Gamemode gamemode;
	private List<Faction> _factions;
	private List<Team> _teams;

	public Human Prefab_Human
	{
		get { return _prefab_human; }
	}
	public Spawn Prefab_Spawn
	{
		get { return _prefab_spawn; }
	}
	public TerrainGenerator Terrain_Generator
	{
		get { return _prefab_terrain_generator; }
	}
	public IList<Team> Teams    //REVIEW: other objects normaly should not have access to this
	{
		get { return _teams.AsReadOnly(); }
	}
	public IList<Faction> Factions
	{
		get { return _factions.AsReadOnly(); }
	}

	private void Awake()
	{
		instance=this;
		_prefab_terrain_generator=Terrain.activeTerrain.GetComponent<TerrainGenerator>();
		_prefab_terrain_generator.AwakeManual();
		game_data =new GameData(this);

		_factions = new List<Faction>() {
			new Faction("ExampleFaction")
		};
		_teams= new List<Team>() { 
			new Team(0, null, Color.red, TerrainGenerator.POSITION_DOCK_SIDE.SOUTH),
			new Team(0, null, Color.blue, TerrainGenerator.POSITION_DOCK_SIDE.NORTH)
		};

		gamemode=new Liquidation(_teams, 1000);

		Human human = Instantiate(_prefab_human);
		human.AwakeManual(_teams[0], "Товарищ",  _factions[0]);
		_teams[0].players.Add(human);

		foreach (Team team in _teams)
		{
			Bot bot = Instantiate(_prefab_bot);
			bot.AwakeManual(team, null, _factions[0]);
			team.players.Add(bot);
		}	
	}
    private void Start()
    {
		_prefab_terrain_generator.StartManual();
		/*foreach(Team team in _teams)
		{
			foreach(Player player in team.players)
			{
				  // pass args
			}
		}*/
    }
    private void Update()
    {
		game_data.ticks++;
		foreach(Team team in _teams)
		{
			foreach(Player player in team.players)
			{
				player.UpdateManual();
			}
		}
		foreach(Team team in _teams)
		{
			foreach(Player player in team.players)
			{
				foreach(Unit unit in player.units)
				{
					unit.UpdateManual();
				}
			}
		}
		/*if(player.goal.is_reached)	// TODO: + stop updating
			ScreenEnd.show();*/
	}
	private void LateUpdate()
	{

	}
	private void FixedUpdate()
	{

	}

	private void OnGUI()
	{

	}
}
