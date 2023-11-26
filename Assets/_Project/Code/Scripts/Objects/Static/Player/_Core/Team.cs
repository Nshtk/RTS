using UnityEngine;
using System.Collections.Generic;
using System;

public class Team
{
	string[] team_names = new string[] {	//UNUSED
		"Свинорез",
		"Товарищи",
		"Пажилые",
		"Сухачи"
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
			//sender.unit
		}
    }
	public string name;
	public TeamGoal goal;
	public int id;
	public Color color;
	public TEAM_TYPE type;
	public List<Player> players=new List<Player>();

	public Team(int id, string name, Color color)
	{
		this.id=id;
		this.color=color;
		this.name=name;
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
