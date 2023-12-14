using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Player Body Parts")]
	public int id;
	public string nickname;
	public Team team;
	public Faction faction;     //TODO: dont store it use gamedata
	public Spawn spawn;

	public int money=0, money_income=5, money_pool = 1000;	//TODO set this by gamemode
	public int difficulty_multiplier;

	public List<Dictionary<int, Unit>> units = new List<Dictionary<int, Unit>>();

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
	}
	protected virtual void Start()
	{
		foreach(Unit unit in faction.units)
		{
			units.Add(new Dictionary<int, Unit>());
		}
	}
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
		foreach (Dictionary<int, Unit> units_of_name in units)
		{
			foreach(Unit unit in units_of_name.Values)
			{
				unit.UpdateManual();
			}
		}
	}
	public virtual bool isAvailableUnit(Unit unit)
	{
		return money>=unit.cost && (unit.limit==-1 || unit.limit>units[id].Count);
	}
	public virtual void buyUnit(int id)
	{
		if (isAvailableUnit(faction.units[id]))
		{
			money-=faction.units[id].cost;
			Unit unit=spawn.spawnUnit(faction.units[id]);
			units[unit.id_in_faction].Add(unit.gameObject.GetInstanceID(), unit);
		}

	}
	public virtual void giveOrder(Vector3 position)
	{
		
	}
}
