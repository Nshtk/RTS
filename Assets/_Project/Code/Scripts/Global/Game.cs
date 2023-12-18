using UnityEngine;
using System;
using System.Collections.Generic;

public class Game : MonoBehaviour	//Class containing main loop
{
	public class GameData          //Class for log/statistics
	{
		public static GameData instance;
		private Game _game;

		public ulong ticks = 0;		// FIXME to BigInt
		public int count_units_died;
		private float time_elapsed_since_update;

		public GameData(Game game)
		{
			instance=this;
			_game=game;
			subscribe();
		}
		private void subscribe()
		{
			Unit.unitSpawned+=handleUnitSpawned;
			Unit.unitDied+=handleUnitDied;
			Flag.flagCaptured+=handleFlagCaptured;
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
		public void handleUnitSpawned(Unit sender, Unit.UnitSpawnedEventArgs e)
		{
			
		}
		public void handleUnitDied(Unit sender, Unit.UnitDiedEventArgs e)
		{
			count_units_died++;
		}
		public void handleFlagCaptured(Flag sender, Flag.FlagCapturedEventArgs e)
		{
			//TODO update teams captured flags here?
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

	public bool is_running=true;		//REVIEW temp

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
			new Team(1, null, Color.red, TerrainGenerator.POSITION_DOCK_SIDE.NORTH)
		};
		gamemode=new Liquidation(_teams, 1000, new Liquidation.LiquidationDifficulty(Gamemode.GamemodeDifficulty.DIFFICULTY_PRESET.NORMAL));

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
		gamemode.setupTeams();
		_prefab_terrain_generator.StartManual();
	}
	private void Update()
	{
		game_data.update();

		foreach(Team team in _teams)
		{
			team.updatePlayers();
			team.goal.update();
			if (team.goal.is_reached)
				is_running=false;		//TODO screen_end.show(); + stop updating
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
		if(!is_running)
			GUI.Label(new Rect(Input.mousePosition.x, Screen.height-Input.mousePosition.y, 200, 200), "GAME HAS ENDED!");
	}
}
