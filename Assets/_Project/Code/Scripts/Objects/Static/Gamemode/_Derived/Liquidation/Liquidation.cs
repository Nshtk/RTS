using System.Collections.Generic;

using Units.Air;

public sealed partial class Liquidation : Gamemode
{
	public new LiquidationDifficulty difficulty;

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

	public override string Description 
	{
		get { return "Each team should score a higher number by killing enemy units!"; }
	}

	public Liquidation(List<Team> teams, int score_max, LiquidationDifficulty difficulty) : base(score_max)
	{
		this.difficulty=difficulty;
		bot_data =new LiquidationBotData(this);
		Count_Teams=teams.Count;
		this.teams = teams;
		this.score_max=score_max;
		//Unit.unitDied+=unitDiedEventHandler;  //REVIEW:
		this.difficulty=difficulty;
	}
	protected override void setupPlayers(List<Player> players)
	{
		foreach (Player player in players)
		{
			player.money_income=difficulty.money_income;
			player.money_pool=difficulty.money_pool;
			player.command_points=difficulty.command_points;
		}
	}
	public override void setupTeams()
	{
		GamemodeGoal[] team_goals = new GamemodeGoal[] {
			new LiquidationGoal(this, "Kill special unit to win faster!")
		};
		foreach(Team team in teams)		//TODO set goals by proportions
		{
			team.setGoal((GamemodeGoal)team_goals[0].Clone(), Team.TEAM_TYPE.TACTICIAN);
			setupPlayers(team.players);
		}
	}

	public override void setGenerationParameters(TerrainGenerator terrain_generator)
	{
		//TODO
	}

	public void handleUnitSpawned(Unit sender, Unit.UnitDiedEventArgs e)
	{
		if (sender is AirUnitExample air_unit_example)
		{
			foreach (Team team in teams)
			{
				if(air_unit_example.owner.team.Id!=team.Id)
					air_unit_example.airUnitExampleDied+=((LiquidationGoal)teams[0].goal).handleAirExampleUnitDied;
			}
		}

	}

	public override Player getWinner()
	{
		throw new System.NotImplementedException();
	}
	/*public void handleUnitDied(Unit sender, Unit.UnitDiedEventArgs e)
{
	teams[sender._player_owner.team.id].goal.score+=sender.cost;
}*/
}
