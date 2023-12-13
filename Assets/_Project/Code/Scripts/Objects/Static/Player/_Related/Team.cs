using UnityEngine;
using System.Collections.Generic;
using System;
using Libraries.Utility;

public class Team
{
	readonly string[] team_names = new string[] {
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
	public void updatePlayers()
	{
		foreach (Player player in players)
		{
			player.UpdateManual();
		}
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
	public bool haveUnit(int count, string property_name, params string[] values) // args find units by such properties as name, cost etc
	{
		foreach(Player player in players)
		{
			foreach(Dictionary<int, Unit> units in player.units)
			{
				foreach(Unit unit in units.Values)
				{
					string value;
					switch (property_name)
					{
						case "type":
						value=unit.type.ToString();
							break;
						case "movement_type":
							value=unit.movement_type.ToString();
							break;
						default:
							value="";
							break;
					}
					for(int i=0; i<values.Length; i++)
					{
						if (values[i]==value)		//REVIEW find by criterias other than unit members?
						{
							count-=1;
							break;
						}
					}
						
					if (count==0)
						return true;
				}
			}
		}
		return false;
	}
}
