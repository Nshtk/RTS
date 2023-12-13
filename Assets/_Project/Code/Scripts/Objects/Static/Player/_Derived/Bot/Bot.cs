using System;
using System.Collections.Generic;
using Libraries.Utility;

using UnityEngine;

public sealed class Bot : Player
{
	public string strategy;
	public string doctrine;
	private Queue<Unit> spawn_buffer=new Queue<Unit>();

	public override void AwakeManual(Team team, string nickname, Faction faction)
	{
		base.AwakeManual(team, nickname, faction);
		if (nickname==null)
			this.nickname=GetInstanceID().ToString();
	}
	protected override void Awake()
	{
		base.Awake();
	}
	protected override void Start()
	{
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}
	public override void giveOrder(Vector3 position)
	{

	}
	private void getPriorityFlag()
	{
	}
	private void getSpecificFlag()
	{
	}
	private Unit setRandomUnitByTypePriority(Dictionary<Unit.UNIT_TYPE, (List<Unit> units, int rate)> units_by_type, int total_rate)
	{
		float random_factor=Mathf.Floor((float)Utility.Random.NextDouble()*total_rate);

		foreach(KeyValuePair<Unit.UNIT_TYPE, (List<Unit> units, int rate)> kv in units_by_type)
		{
			random_factor-=kv.Value.rate;
			if(random_factor<1)
				return units_by_type[kv.Key].units[Utility.Random.Next(0, units_by_type[kv.Key].units.Count)];
		}

		return null;
	}
	public void getUnitToSpawn()
	{
		//setRandomUnitByTypePriority();
	}
}
