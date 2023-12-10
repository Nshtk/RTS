using System;
using System.Collections.Generic;

public sealed class Liquidation : Gamemode
{
	/*public override int Count_Teams
	{
		get { return _count_teams; }
		protected set
		{
			if(value>10)
				_count_teams=10;
			else
				_count_teams=value;
		}
	}*/

	public Liquidation(List<Team> teams, int score_max) : base(score_max)
	{
		description="Each team should score a higher number by killing enemy units!";
		//Count_Teams=count_teams;
		this.teams = teams;
		this.score_max=score_max;
	}
	public override void setTeams()
	{
		Team.TeamGoal[] team_goals = new Team.TeamGoal[] {
			new Team.TeamGoal("Kill them all!")
		};
		Action[] actions = new Action[] {	//REVIEW:
			() => { Unit.unitDied+=team_goals[0].unitDiedEventHandler; }
		};
		foreach(Team team in teams)		//TODO set goals by proportions
		{
			team.setGoal(team_goals[0], actions[0], Team.TEAM_TYPE.TACTICIAN, score_max/teams.Count);
		}
	}
	public override void updateTeamGoals()
	{
        foreach (Team team in teams)
        {
            
        }
    }

	public override void setGenerationParameters(TerrainGenerator terrain_generator)
	{
		
	}

	/*public int minutes = 1;

private float timeLeft = 0.0f;

void Awake()
{
	timeLeft = minutes * 60;
}

void Update()
{
	timeLeft -= Time.deltaTime;
}

public override string GetDescription()
{
	return "Survival";
}

public override bool GameFinished()
{
	foreach(Player player in players)
	{
		if(player && player.human && player.IsDead())
			return true;
	}
	return timeLeft < 0;
}

public override bool PlayerMeetsConditions(Player player)
{
	return player && player.human && !player.IsDead();
}*/
}
