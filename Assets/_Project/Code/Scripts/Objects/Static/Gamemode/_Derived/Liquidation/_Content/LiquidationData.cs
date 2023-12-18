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

			public LiqudationStrategy()
			{

			}


			public override DynamicObject getPriorityTarget(int total_rate)
			{
				throw new NotImplementedException();
			}

			public override Vector3 getPriorityDestination(int total_rate)
			{
				throw new NotImplementedException();
			}

			public override float getConfidenceCurrent()
			{
				//if()
				return 1f;
			}

			protected override void updateDestinations()
			{
				throw new NotImplementedException();
			}

			protected override void updateTargets()
			{
				throw new NotImplementedException();
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
				new LiqudationStrategy(),
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
