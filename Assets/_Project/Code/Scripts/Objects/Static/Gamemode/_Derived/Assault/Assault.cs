using UnityEngine;

public class Assault : Gamemode
{
	public override int Count_Teams
	{
		get { return _count_teams; }
		protected set
		{
			if (value>10)
				_count_teams=10;
			else
				_count_teams=value;
		}
	}

	public Assault(int score_max) : base(score_max)
	{
	}

	public override void setGenerationParameters(TerrainGenerator terrain_generator)
	{
		throw new System.NotImplementedException();
	}

	public override void setTeams()
	{
		throw new System.NotImplementedException();
	}

	public override void updateTeamGoals()
	{
		throw new System.NotImplementedException();
	}

	/*public override string GetDescription()
	{
		return "Conquest";
	}
	
	public override bool GameFinished()
	{
		if(players == null)
			return true;
		int playersLeft = players.Length;
		foreach(Player player in players)
		{
			if(!PlayerMeetsConditions(player))
				playersLeft--;
		}
		return playersLeft == 1;
	}
	
	public override bool PlayerMeetsConditions(Player player)
	{
		return player && !player.IsDead();
	}*/
}
