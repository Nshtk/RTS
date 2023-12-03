using UnityEngine;
using System;
using System.Collections.Generic;


public class Game : MonoBehaviour	//Class containing main loop.
{
	public class GameData           //TODO: add Debug class? //Class for statistics
	{
		public static GameData instance;
		private Game _game;

		public Human prefab_human;
		public Spawn prefab_spawn;

		public ulong ticks = 0;		// FIXME: to BigInt
		public int count_units_died;

		public IList<Team> Teams	//REVIEW: other objects normaly should not have access to this
		{
			get { return _game._teams.AsReadOnly(); }
		}
		public IList<Faction> Factions
		{
			get { return _game._factions.AsReadOnly(); }
		}

		public GameData(Game game)
		{
			instance=this;
			_game=game;
			prefab_human=game._human_prefab;
			prefab_spawn=game._spawn_prefab;
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

	[SerializeField] private Human _human_prefab;
	[SerializeField] public Spawn _spawn_prefab;	
	//[SerializeField] private Bot _bot_prefab;

	public GameData game_data;
	private Gamemode gamemode;
	private List<Faction> _factions;
	private List<Team> _teams;
	private TerrainGenerator terrain_generator;

	private void Awake()
	{
		terrain_generator=Terrain.activeTerrain.GetComponent<TerrainGenerator>();
		terrain_generator.AwakeManual();
		game_data =new GameData(this);
		_teams= new List<Team>() { 
			new Team(0, "Свинорез", Color.red)
		}; 
		_factions = new List<Faction>() {
			new Faction("ExampleFaction")
		};
		gamemode=new Liquidation(_teams, 1000);
		foreach(Team team in _teams)
		{
			Player player = Instantiate(_human_prefab);
			player.AwakeManual("Товарищ", team, _factions[0]);
			team.players.Add(player);
		}	
	}
    private void Start()
    {
		terrain_generator.StartManual();
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
