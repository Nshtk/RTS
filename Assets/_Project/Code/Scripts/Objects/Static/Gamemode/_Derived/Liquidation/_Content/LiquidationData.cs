using System;
using System.Collections.Generic;
using System.Linq;

using Units.Air;

using UnityEngine;

public sealed partial class Liquidation
{
	public sealed class LiquidationGoal : GamemodeGoal
	{
		new Liquidation gamemode;

		public LiquidationGoal(Liquidation gamemode, string description) : base(gamemode, description)
		{

		}

		public override bool update()
		{
			if (score>gamemode.score_max)
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

			public override void updatePriorities()
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

		public LiquidationBotData()
		{
			strategies=new LiqudationStrategy[] {
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
