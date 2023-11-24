using UnityEngine;
using System;
using System.Collections.Generic;
using Units.Ground;
using Units.Air;

public class Game : MonoBehaviour	//Class containing main loop.
{
	public class GameData
	{
		//TODO: add statistics
		//TODO: add Debug class?
		public static GameData _instance;
		private Game _game;

		public ulong ticks = 0; // FIXME: to BigInt

		public IList<Player> Players
		{
			get { return _game._players.AsReadOnly(); }
		}
		public IList<Faction> Factions
		{
			get { return _game._factions.AsReadOnly(); }
		}

		public GameData(Game game)
		{
			_instance=this;
			_game=game;
		}
	}

	[SerializeField] private Human _human_prefab;
	//[SerializeField] private Bot _bot_prefab;

	public GameData game_data;
	private Gamemode gamemode;
	private List<Player> _players = new List<Player>(1); //NOTE: initial capacity reduces memory reallocations
	private List<Faction> _factions;

	private void Awake()
	{
		game_data=new GameData(this);
		gamemode=new Liquidation();
		/*new List<Faction> {	//TODO: to game data?
			new Faction("A", null, new List<Unit> {
				new GroundExample(),
				new AirExample()
			})
		};*/
		var grd = new List<GroundUnit>(Resources.LoadAll<GroundUnit>(""));
		var air = new List<AirUnit>(Resources.LoadAll<AirUnit>(""));
		List<Unit> units=new List<Unit>();
		units.AddRange(grd);
		units.AddRange(air);
		_factions = new List<Faction>() {
			new Faction("A", null, units)
		};
		for(int i = 0; i<_players.Capacity; i++)
		{
			//Human human = gameObject.AddComponent<Human>();
			_players.Add(Instantiate(_human_prefab));
		}
	}
    private void Start()
    {
		foreach(Player player in _players)
		{
			player.StartManual(_factions[0]);   // pass args
		}
    }
    private void Update()
    {
		game_data.ticks++;
        foreach(Player player in _players)
		{
			player.UpdateManual();
		}
		foreach(Player player in _players)
		{
			foreach(Unit unit in player.units)
			{
				unit.UpdateManual(); 
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
