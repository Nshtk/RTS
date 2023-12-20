using Libraries.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

using Units.Air;

using UnityEngine;

public sealed partial class Liquidation
{
	public sealed class LiquidationDifficulty : GamemodeDifficulty
	{
		public LiquidationDifficulty(DIFFICULTY_PRESET preset) : base(preset)
		{
			switch(preset)
			{
				case DIFFICULTY_PRESET.CUSTOM:
					break;
				case DIFFICULTY_PRESET.EASY:
					break;
				case DIFFICULTY_PRESET.NORMAL:
					money_income = 5; money_pool = 1000;
					command_points = 200;
					break;
				case DIFFICULTY_PRESET.HARD:
					break;
				case DIFFICULTY_PRESET.VERY_HARD:
					break;
				case DIFFICULTY_PRESET.TEST:
					break;
				default:
					break;
			}
		}
	}

	public sealed class LiquidationGoal : GamemodeGoal
	{
		private new Liquidation _gamemode;

		public LiquidationGoal(Liquidation gamemode, string description) : base(gamemode, description)
		{
			_gamemode=gamemode;
		}

		public override bool update()
		{
			if (score>_gamemode.score_max)
				return is_reached = true;
			return false;
		}

		public void handleAirExampleUnitDied(AirUnitExample sender, AirUnitExample.AirUnitExampleDiedEventArgs e)	//TEMP kill this special unit and goal will be reached
		{
			is_reached = true;
		}
	}
	public sealed class LiquidationBotData : GamemodeBotData
	{
		public class LiqudationStrategy : Strategy
		{
			public List<(Unit, int)> Targets
			{
				get
				{
					return _targets.Cast<(Unit, int)>().ToList();
				}
			}

			public LiqudationStrategy(float confidence)
			{
				_confidence = confidence;
			}

			protected override void updateConfidence()
			{
				//if(team_my.units.count<5)		//TODO
				_confidence=0.1f;
			}

			public override DynamicObject getPriorityTarget(int total_rate)
			{
				var team_enemy_id = Utility.Random.Next(teams_enemy.Count);     //TEMP
				var player_enemy_id = Utility.Random.Next(teams_enemy[team_enemy_id].players.Count);
				List<Unit> units = new();
				foreach(var dict_units in teams_enemy[team_enemy_id].players[player_enemy_id].units_by_id_in_faction_id_unit)
				{
					foreach(var kv in dict_units)
					{
						units.Add(kv.Value);
					}
				}
				if (units.Count==0)
					return null;
				return units[Utility.Random.Next(units.Count)];
			}

			public override Vector3 getPriorityDestination(int total_rate)
			{
				return Utility.getRangedVector3(0, 256, 0,0,0,256);
			}
			protected override void updateDestinations()
			{

			}

			protected override void updateTargets()
			{

			}

			public override object Clone()
			{
				return new LiqudationStrategy(_confidence);
			}
		}
		public sealed class LiqudationDoctrine : Doctrine
		{
			public LiqudationDoctrine(Dictionary<Unit.UNIT_TYPE, (int priority, int limit, Func<int, int> updatePriority)> unit_types_data=null)
			{
				if(unit_types_data!=null)
				{
					foreach (KeyValuePair<Unit.UNIT_TYPE, (int, int, Func<int, int>)> kv in unit_types_data)
					{
						Unit_Types_Data[kv.Key]=kv.Value;
					}
				}
			}
		}

		private new Liquidation _gamemode;

		public LiquidationBotData(Liquidation liquidation) : base(liquidation)
		{
			_gamemode=liquidation;

			strategies =new LiqudationStrategy[] {
				new LiqudationStrategy(0.5f),
			};
			doctrines=new Doctrine[] {
				new LiqudationDoctrine(),	//Universal doctrine
				new LiqudationDoctrine(new Dictionary<Unit.UNIT_TYPE, (int priority, int limit, Func<int, int> updatePriority)> { //Heavy doctrine
					{ Unit.UNIT_TYPE.INFANTRY_TACTICAL, (5, -1, 
					priority =>
					priority+=2)}
				}),	
			};
		}
	}
}
