using System;
using System.Collections.Generic;
using Libraries.Utility;

using UnityEngine;

public sealed class Bot : Player
{
	public Gamemode.GamemodeBotData.Strategy strategy;
	public Gamemode.GamemodeBotData.Doctrine doctrine;

	private Queue<Unit> unit_spawn_buffer=new Queue<Unit>();
	private Unit unit_to_spawn;
	private Dictionary<Unit.UNIT_TYPE, (List<Unit> units, int rate)> units_by_type_available;

	protected override void Awake()
	{
		base.Awake();
		units_by_type_available=new Dictionary<Unit.UNIT_TYPE, (List<Unit> units, int rate)>();
	}
	public override void AwakeManual(Team team, string nickname, Faction faction)
	{
		base.AwakeManual(team, nickname, faction);
		if (nickname==null)
			this.nickname=GetInstanceID().ToString();

		doctrine=Game.instance.gamemode.bot_data.doctrines[Utility.Random.Next(Game.instance.gamemode.bot_data.doctrines.Length)];
		Dictionary<Gamemode.GamemodeBotData.Doctrine, float> doctrine_chance = new();
		float chance_reduction = 0.5f/team.players.Count;	//FIXME bots.Count
		foreach (var doctrine in Game.instance.gamemode.bot_data.doctrines)
			doctrine_chance.Add(doctrine, 1f);
		foreach (Player player in team.players)
		{
			if(player is Bot bot)
			{
				if(bot.doctrine!=null)
				{
					doctrine_chance[bot.doctrine]-=chance_reduction;
					if (doctrine==bot.doctrine)
						if (Utility.Random.Next()>doctrine_chance[bot.doctrine])
							doctrine=Game.instance.gamemode.bot_data.doctrines[Utility.Random.Next(Game.instance.gamemode.bot_data.doctrines.Length)];
				}
				
			}
		}
		foreach(Unit.UNIT_TYPE unit_type in faction.unit_types)
		{
			units_by_type_available.Add(unit_type, (new List<Unit>(), 0));
		}
	}
	protected override void Start()
	{
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}
	public override void UpdateManual()
	{ 
		base.UpdateManual();
		if(Game.instance.game_data.ticks%1000==0)
		{
			if (unit_spawn_buffer.Count>0)
				buyUnit(unit_spawn_buffer.Dequeue().id_in_faction);
			else
				getUnitToSpawn();       //TODO accumulate money instead of buying if not in danger
		}
	}

	public override bool isAvailableUnit(Unit unit)	
	{
		return base.isAvailableUnit(unit);	//TODO | Game.instance.game_data.time_elapsed>value
	}
	public override void giveOrder(Vector3 position)
	{
		//Game.instance.gamemode.bot_data.strategies
	}
	public override void buyUnit(int id)
	{
		base.buyUnit(id);
	}
	private Unit getRandomUnitByPriority(int total_rate)
	{
		float random_factor=Mathf.Floor((float)Utility.Random.NextDouble()*total_rate);

		foreach(KeyValuePair<Unit.UNIT_TYPE, (List<Unit> units, int rate)> kv in units_by_type_available)
		{
			random_factor-=kv.Value.rate;
			if(random_factor<1)
				return units_by_type_available[kv.Key].units[Utility.Random.Next(0, units_by_type_available[kv.Key].units.Count)];
		}

		return null;
	}
	public void getUnitToSpawn()
	{
		units_by_type_available.Clear();

		int total_rate = 0;
		if (team.type==Team.TEAM_TYPE.TACTICIAN) 
		{
			foreach(Unit unit in faction.units)
			{
				if(isAvailableUnit(unit))
				{
					if (!units_by_type_available.ContainsKey(unit.type))
					{
						units_by_type_available.Add(unit.type, (new List<Unit>(), 0));
					}
					units_by_type_available[unit.type].units.Add(unit);
					//units_by_type[unit.type].rate+=unit.priority;	//TODO
					//total_rate+=unit.priority;
				}
			}
		}
		if (units_by_type_available.Count==0)
			return;
		unit_to_spawn=getRandomUnitByPriority(total_rate);
		unit_spawn_buffer.Enqueue(unit_to_spawn);
	}
}
