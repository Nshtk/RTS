using UnityEngine;
using System.Collections.Generic;
using System;
using Libraries;

public class Team
{
	readonly string[] team_names = new string[] {	//UNUSED
		"Свинорез",
		"Товарищи",
		"Пажилые",
		"Сухачи",
		"Першiе"
	};
	public enum TEAM_TYPE
	{
		ATACKER,
		DEFENDER,
		TACTICIAN		// Free acting team
	}
	public class TeamGoal
	{
		public int score;
		public bool is_reached=false;
		public string description;
		public Action setScore;

        public TeamGoal(string description)
        {
			this.description=description;
		}

		public void unitDiedEventHandler(Unit sender, Unit.UnitDiedEventArgs e)
		{
			score+=sender.cost;
		}
    }
	public string name;
	public TeamGoal goal;
	public int id;
	public Color color;
	public TEAM_TYPE type;
	public List<Player> players=new List<Player>();
	public TerrainGenerator.POSITION_DOCK_SIDE position_dock_side;

	public Team(int id, string? name=null, Color? color=null, TerrainGenerator.POSITION_DOCK_SIDE? position_dock_side=null)
	{
		this.id=id;
		if(name==null)
			this.name=team_names[Utility.Random.Next(0, team_names.Length)];
		else
			this.name=name;
		if (color==null)
			this.color=Color.white;
		else
			this.color=color.Value;
		this.position_dock_side=position_dock_side.Value;
	}

	public void setGoal(TeamGoal goal, Action subscribe, TEAM_TYPE type, int score=0)
	{
		this.goal=goal;
		this.goal.score = score;
		this.type=type;
		subscribe();
	}
	public void getUnitInfo()
	{
	
	}
	public void haveUnit() // args find units by such properties as name, cost etc
	{

	}
	public void getFlagUnits()
	{

	}
}
