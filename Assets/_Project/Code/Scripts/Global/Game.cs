using UnityEngine;
using System;
using System.Collections.Generic;
using Libraries.Terrain;


public class Game : MonoBehaviour	//Class containing main loop
{
	public class GameData          //Class for statistics
	{                              //TODO event aggregator (e.g. flag captured -> update team's captured flags)  
		public static GameData instance;
		private Game _game;

		public ulong ticks = 0;		// FIXME to BigInt
		public int count_units_died;
		private float time_elapsed_since_update;

		public GameData(Game game)
		{
			instance=this;
			_game=game;
		}

		public void update() 
		{
			time_elapsed_since_update += Time.deltaTime;
			if (time_elapsed_since_update>0.01f)
			{
				time_elapsed_since_update=0f;
				ticks++;
			}
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
	public Gamemode gamemode;
	private List<Faction> _factions;
	private List<Team> _teams;



	public LayerMask Layer_Mask_Terrain		//Singleton structs
	{
		get;
		private set;
	}
	public LayerMask Layer_Mask_Human_Order
	{
		get;
		private set;
	}

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
	public IList<Team> Teams
	{
		get { return _teams.AsReadOnly(); }
	}
	public IList<Faction> Factions
	{
		get { return _factions.AsReadOnly(); }
	}

	private void initialise() 
	{
		instance=this;
		game_data=new GameData(this);
		_prefab_terrain_generator=Terrain.activeTerrain.GetComponent<TerrainGenerator>();
		_prefab_terrain_generator.AwakeManual();

		Layer_Mask_Terrain=1<<LayerMask.NameToLayer("Terrain");
		Layer_Mask_Human_Order=Layer_Mask_Terrain | 1<<LayerMask.NameToLayer("PlaneRaycast") | 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Unit");
	}

	private void Awake()
	{
		initialise();

		_factions = new List<Faction>() {
			new Faction("ExampleFaction")
		};
		_teams= new List<Team>() { 
			new Team(0, null, Color.blue, TerrainGenerator.POSITION_DOCK_SIDE.SOUTH),
			new Team(0, null, Color.red, TerrainGenerator.POSITION_DOCK_SIDE.NORTH)
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
		game_data.update();

		foreach(Team team in _teams)
		{
			team.updatePlayers();
			/*if(team.goal.is_reached)	// TODO + stop updating
				screen_end.show();*/
		}
		foreach (Team team in _teams)
		{
			foreach(Player player in team.players)
			{
				player.updateUnits();
			}
		}
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
