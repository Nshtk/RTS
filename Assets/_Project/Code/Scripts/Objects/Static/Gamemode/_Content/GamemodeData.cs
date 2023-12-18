using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class Gamemode
{
	public abstract class GamemodeDifficulty
	{
		public enum DIFFICULTY_PRESET
		{
			CUSTOM,
			EASY,
			NORMAL,
			HARD,
			VERY_HARD,
			TEST
		}

		public DIFFICULTY_PRESET preset;
		public int money_income=999999, money_pool=999999;
		public int command_points=999999;

		public GamemodeDifficulty(DIFFICULTY_PRESET preset)
		{
			this.preset = preset;
		}
	}
	public abstract class GamemodeGoal : ICloneable //REVIEW to struct?
	{
		protected Gamemode _gamemode;
		public int score;
		public bool is_reached = false;
		public string description;
		//public Action setScore;

		public GamemodeGoal(Gamemode gamemode, string description)
		{
			_gamemode = gamemode;
			this.description=description;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public abstract bool update();
	}
	public abstract class GamemodeBotData
	{
		public abstract class Strategy
		{
			GamemodeBotData bot_data;
			protected List<(DynamicObject dynamic_object, int rate)> _targets=new List<(DynamicObject dynamic_object, int rate)>();
			protected List<(Vector3, int rate)> _destinations = new List<(Vector3, int rate)>();

			public Strategy()
			{ }

			public virtual void update()
			{
				updateDestinations();
				updateTargets();
			}

			protected abstract void updateDestinations();
			protected abstract void updateTargets();

			//public abstract void updatePriorities();
			public abstract DynamicObject getPriorityTarget(int total_rate);	//TODO to one generic function in Utility?
			public abstract Vector3 getPriorityDestination(int total_rate);
			public abstract float getConfidenceCurrent();
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

		protected Gamemode _gamemode;
		public Strategy[] strategies;
		public Doctrine[] doctrines;

		public GamemodeBotData(Gamemode gamemode)
		{ 
			_gamemode = gamemode;
		}
	}
}
