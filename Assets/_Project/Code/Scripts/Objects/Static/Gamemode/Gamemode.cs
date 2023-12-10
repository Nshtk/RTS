using System.Collections.Generic;
using UnityEngine;

public abstract class Gamemode		//TODO: nested class difficulty
{
	public int score_max;
	public string description;
	public List<Team> teams;
	public readonly int count_teams_max;

	/*protected int _count_teams;
	public abstract int Count_Teams
	{
		get;
		protected set;
	}*/

	protected Gamemode(int score_max)
    {
		//Game.GameData.instance;
		this.score_max=score_max;
    }
	public abstract void setTeams();
	public abstract void updateTeamGoals();
	public abstract void setGenerationParameters(TerrainGenerator terrain_generator);
    /*protected Player[] players;

	public void SetPlayers(Player[] players)
	{
		this.players = players;
	}

	public Player[] GetPlayers()
	{
		return players;
	}

	public virtual bool GameFinished()
	{
		if(players == null)
			return true;
		foreach(Player player in players)
		{
			if(PlayerMeetsConditions(player))
				return true;
		}
		return false;
	}

	public Player GetWinner()
	{
		if(players == null)
			return null;
		foreach(Player player in players)
		{
			if(PlayerMeetsConditions(player))
				return player;
		}
		return null;
	}

	public abstract string GetDescription();

	public abstract bool PlayerMeetsConditions(Player player);*/
}
