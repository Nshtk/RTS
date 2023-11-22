using UnityEngine;

public class Gamemode : MonoBehaviour
{
    public struct Goal
    {
        public bool is_reached;
        public string description;
    }

    public Goal goal;

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
