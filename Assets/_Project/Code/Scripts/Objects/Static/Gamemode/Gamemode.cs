using System.Collections.Generic;
using UnityEngine;

public abstract partial class Gamemode
{
	public GamemodeBotData bot_data;
	public int score_max;
	public string description;
	public List<Team> teams;
	public readonly int count_teams_max;
	protected int _count_teams;

	protected float time_left = 0f;

	public abstract string Description
	{
		get;
	}

	public abstract int Count_Teams
	{
		get;
		protected set;
	}

	protected Gamemode(int score_max)
    {
		this.score_max=score_max;
		time_left = 60f*60f;
	}
	public abstract void setTeams();
	protected virtual bool updateTime()
	{
		time_left -= Time.deltaTime;
		return time_left<0;
	}
	protected virtual bool updateTeamGoals()
	{
		foreach (Team team in teams)
		{
			team.goal.update();
			if (team.goal.is_reached)
			{
				return true;
			}
		}
		return false;
	}
	public virtual void update()
	{
		if(updateTime() || updateTeamGoals())
			getWinner();
	}
	public abstract void setGenerationParameters(TerrainGenerator terrain_generator);
	public abstract Player getWinner();
}
