using UnityEngine;
using System;
using System.Collections.Generic;

public class MainLoop : MonoBehaviour
{
	[SerializeField] private Gamemode gamemode;

	[SerializeField] private Human _human_prefab;
	//[SerializeField] private Bot _bot_prefab;
	private List<Player> players=new List<Player>(1);	//NOTE: initial capacity reduces memory reallocations
	private List<Faction> _factions=new List<Faction>();

	private void Awake()
	{
		for(int i=0; i<players.Capacity; i++)
		{
			Human human = Instantiate(_human_prefab);
			players.Add(human);
		}
	}
    private void Start()
    {
		foreach(Player player in players)
		{
			player.StartManual();	// pass args
		}
    }
    private void Update()
    {
		Game.ticks++;
        foreach(Player player in players)
		{
			player.UpdateManual();
		}
		foreach(Player player in players)
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
