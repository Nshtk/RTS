using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flag : Structure
{
	public enum FLAG_STATUS
	{
		CLEAR,
		DEFENDED,
		DEFENDED_STRONG,
		ATTACKED,
		ATTACKED_STRONG
	}

	public FLAG_STATUS status;
	public Team owner;
	public HashSet<Unit> flag_units;
	private float _progress=0;
	Dictionary<int, int> flag_units_team_id_units_count;

	public float Progress
	{
		get { return _progress; }
		private set 
		{
			if (value<0f)
				_progress = 0f;
			else if(value>100f)
				_progress = 100f;
			else
				_progress = value;
		}
	}

	public void initialise(Vector3 size)
	{
		transform.localScale = size;
	}
	protected override void Awake()
	{
		flag_units=new HashSet<Unit>();
		Unit.unitDied+=handleUnitDied;
		foreach (Team team in Game.instance.Teams)
			flag_units_team_id_units_count.Add(team.Id, 0);
	}
	protected override void Start()
	{
		base.Start();
		_progress=0;
	}
	protected override void Update()
	{
		base.Update();
	}
	public override void UpdateManual()
	{
		if (_progress==100f)
		{
			flagCaptured?.Invoke(this, new FlagCapturedEventArgs($"{Id} has been captured."));
			foreach (Unit unit in flag_units)
			{
				if (unit.state==Unit.UNIT_STATE.IDLE)
					unit.status=Unit.UNIT_STATUS.WAITING;
			}
		}
		else if (_progress==0)
			owner=null;

		if (Game.instance.game_data.ticks==50)
			updateFlagUnits();
		if (Game.instance.game_data.ticks==100)
			updateStatus();

	}
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null) 
		{
			flag_units_team_id_units_count[unit.owner.team.Id]+=1;
			flag_units.Add(unit);
		}
	}
	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null)
		{
			if(flag_units.Contains(unit))
			{
				flag_units_team_id_units_count[unit.owner.team.Id]-=1;
				flag_units.Remove(unit);
			}
		}
	}
	public void updateStatus()
	{
		if(flag_units.Count!=0)
		{
			foreach (Unit unit in flag_units)
			{
				if (unit.status==Unit.UNIT_STATUS.WAITING && _progress!=100f)
					unit.status=Unit.UNIT_STATUS.WORKING;
			}
		}
		else
			status = FLAG_STATUS.CLEAR;
		
	}
	public void updateFlagUnits()
	{
		Collider[] colliders = Physics.OverlapBox(GetComponent<Renderer>().bounds.center, transform.localScale);
		foreach (Collider collider in colliders)
		{
			if(collider.gameObject.GetComponent<Unit>() is Unit unit)
			{
				if (!flag_units.Contains(unit))
					flag_units.Add(unit);
				if (unit.status==Unit.UNIT_STATUS.WAITING && _progress!=100f)
					unit.status=Unit.UNIT_STATUS.WORKING;
			}

		}
	}
	public List<Unit> getFlagUnits()
	{
		return flag_units.ToList();
	}
	public FLAG_STATUS getFlagStatus(int id_team)
	{
		int units_ratio = flag_units_team_id_units_count[id_team]-(flag_units.Count-flag_units_team_id_units_count[id_team]);
		if(id_team==owner.Id)	
		{
			if (units_ratio>2)	//TODO evaluate ration in % of total count of units in team
				return FLAG_STATUS.ATTACKED_STRONG;
			else if (units_ratio>0)
				return FLAG_STATUS.ATTACKED;
			else if(units_ratio<0)
				return FLAG_STATUS.DEFENDED;
			else
				return FLAG_STATUS.CLEAR;
		}
		else if(id_team!=owner.Id)  //REVIEW
		{
			if (units_ratio>2)
				return FLAG_STATUS.ATTACKED_STRONG;
			else if (units_ratio>0)
				return FLAG_STATUS.ATTACKED;
			else if (units_ratio<0)
				return FLAG_STATUS.DEFENDED;
			else
				return FLAG_STATUS.CLEAR;
		}
		else
		{
			if(units_ratio!=0)
				return FLAG_STATUS.DEFENDED;
			else
				return FLAG_STATUS.CLEAR;
		}

	}
	public void handleUnitDied(Unit sender, Unit.UnitDiedEventArgs e)
	{
		flag_units.Remove(sender);
	}

	public delegate void FlagCapturedEventHandler(Flag sender, FlagCapturedEventArgs e);
	public static event FlagCapturedEventHandler flagCaptured;
	public class FlagCapturedEventArgs : EventArgs
	{
		public string Message
		{
			get;
			set;
		}
		public FlagCapturedEventArgs(string message)
		{
			Message = message;
		}
	}
}
