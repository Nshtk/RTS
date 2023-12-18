using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Player properties")]
	public int id;
	public string nickname;
	public Team team;
	public Faction faction;     //TODO: dont store it, use gamedata?
	public Spawn spawn;

	public int money, money_income, money_pool;
	public int command_points;

	public List<Dictionary<int, Unit>> units_by_id_in_faction_id_unit = new List<Dictionary<int, Unit>>();

	protected virtual void Awake()
	{
	}
	public virtual void AwakeManual(Team team, string? nickname, Faction faction)
	{
		spawn=Instantiate(Game.instance.Prefab_Spawn); spawn.AwakeManual(this, team.position_dock_side);
		this.team = team;
		if (nickname==null)
			this.nickname ="Unknown";
		else
			this.nickname=nickname;
		this.nickname = nickname;
		this.faction=faction;
		foreach (Unit unit in faction.units)
		{
			units_by_id_in_faction_id_unit.Add(new Dictionary<int, Unit>());
		}
	}
	protected virtual void Start()
	{}
	protected virtual void Update()
	{

	}
	public virtual void UpdateManual()
	{
		if (money_pool>=money_income)
		{
			money+=money_income;
			money_pool-=money_income;
		}
		else
		{
			money+=money_pool;
			money_pool=0;
		}
	}
	public virtual void updateUnits()
	{
		foreach (Dictionary<int, Unit> units_of_name in units_by_id_in_faction_id_unit)
		{
			foreach(Unit unit in units_of_name.Values)
			{
				unit.UpdateManual();
			}
		}
	}
	public virtual bool isAvailableUnit(Unit unit)
	{
		return money>=unit.cost_money && command_points>=unit.cost_command_points && (unit.limit==-1 || unit.limit>units_by_id_in_faction_id_unit[id].Count);
	}
	public virtual void buyUnit(int id)
	{
		if (isAvailableUnit(faction.units[id]))
		{
			money-=faction.units[id].cost_money;
			Unit unit=spawn.spawnUnit(faction.units[id]);
			units_by_id_in_faction_id_unit[unit.id_in_faction].Add(unit.gameObject.GetInstanceID(), unit);
		}

	}
	public virtual void giveOrder(Vector3 position)
	{
		
	}
}
