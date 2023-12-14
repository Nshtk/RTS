using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class Gamemode
{
	public abstract class GamemodeBotData
	{
		public abstract class Strategy
		{
			protected List<(DynamicObject dynamic_object, int rate)> _targets=new List<(DynamicObject dynamic_object, int rate)>();
			protected List<(Vector3, int rate)> _destinations = new List<(Vector3, int rate)>();

			public Strategy()
			{

			}

			public abstract void updatePriorities();
			public abstract DynamicObject getPriorityTarget(int total_rate);	//TODO to one generic function in Utility?
			public abstract Vector3 getPriorityDestination(int total_rate);
		}
		public class Doctrine
		{
			protected Func<int, int> getPriorityDefault = (priority => priority);
			protected Func<int, int> getPriorityInfantry = (priority => priority);
			protected Func<int, int> getPriorityVehicle = (priority => priority);
			protected Func<int, int> getPriorityHelicopter = (priority => priority);
			protected Func<int, int> getPriorityAirplane = (priority => priority);

			public Dictionary<Unit.UNIT_TYPE, (int priority, int limit, Func<int, int> updatePriority)> Unit_Types_Data
			{
				get;
				protected set;
			}

			public Doctrine()
			{
				Unit_Types_Data=Unit_Types_Data=new Dictionary<Unit.UNIT_TYPE, (int priority, int limit, Func<int, int> updatePriority)> {
					{Unit.UNIT_TYPE.INFANTRY_COMMON,    (2, -1, getPriorityInfantry )},
					{Unit.UNIT_TYPE.INFANTRY_TACTICAL,  (3, -1, getPriorityInfantry )},
					{Unit.UNIT_TYPE.INFANTRY_VANGUARD,  (3, -1, getPriorityInfantry )},
					{Unit.UNIT_TYPE.INFANTRY_SABOTEUR,  (1, -1, getPriorityInfantry )},
					{Unit.UNIT_TYPE.INFANTRY_SUPPORT,   (2, -1, getPriorityInfantry )},
					{Unit.UNIT_TYPE.VEHICLE_COMMON,     (3, -1, getPriorityVehicle )},
					{Unit.UNIT_TYPE.VEHICLE_LIGHT,      (4, -1, getPriorityVehicle )},
					{Unit.UNIT_TYPE.VEHICLE_HEAVY,      (5, -1, getPriorityVehicle )},
					{Unit.UNIT_TYPE.VEHICLE_ARTILLERY,  (4, -1, getPriorityVehicle )},
					{Unit.UNIT_TYPE.VEHICLE_SUPPORT,    (3, -1, getPriorityVehicle )},
					{Unit.UNIT_TYPE.HELICOPTER_SCOUT,   (4, -1, getPriorityHelicopter )},
					{Unit.UNIT_TYPE.HELICOPTER_ATTACK,  (5, -1, getPriorityHelicopter )},
					{Unit.UNIT_TYPE.AIRPLANE_FIGHTER,   (4, -1, getPriorityAirplane )},
					{Unit.UNIT_TYPE.AIRPLANE_BOMBER,    (5, -1, getPriorityAirplane )},
					{Unit.UNIT_TYPE.ULTIMATE,           (8, -1, getPriorityDefault )}
				};
			}
		}

		public Strategy[] strategies;
		public Doctrine[] doctrines;

		public GamemodeBotData()
		{ }
	}
	public abstract class GamemodeDifficulty
	{
		public GamemodeDifficulty()
		{ }
	}
	public abstract class GamemodeGoal
	{
		protected Gamemode gamemode;
		public int score;
		public bool is_reached = false;
		public string description;
		public Action setScore;

		public GamemodeGoal(Gamemode gamemode, string description)
		{
			this.gamemode = gamemode;
			this.description=description;
		}

		public abstract bool update();
	}
}
