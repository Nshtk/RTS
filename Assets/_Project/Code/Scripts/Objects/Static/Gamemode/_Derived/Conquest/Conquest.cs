using UnityEngine;

public class Conquest : Gamemode
{
	/*public class ConquestBotData : GamemodeBotData
	{
		public class ConquestStrategy : Strategy 
		{
			public override float getConfidenceCurrent()
			{
				throw new System.NotImplementedException();
			}

			public override Vector3 getPriorityDestination(int total_rate)
			{
				throw new System.NotImplementedException();
			}

			public override DynamicObject getPriorityTarget(int total_rate)
			{
				throw new System.NotImplementedException();
			}

			public override void updatePriorities()
			{
				throw new System.NotImplementedException();
			}

			private void getPriorityFlag(int total_rate)
			{
				foreach (Flag flag in Game.instance.Terrain_Generator.map.flags)
				{

				}
			}
			private void getSpecificFlag()
			{
			}
		}
	}*/
	public override int Count_Teams
	{
		get { return _count_teams; }
		protected set
		{
			if (value>10)
				_count_teams=10;
			else
				_count_teams=value;
		}
	}

	public override string Description { get; }

	public Conquest(int score_max) : base(score_max)
	{
	}

	public override void setGenerationParameters(TerrainGenerator terrain_generator)
	{
		throw new System.NotImplementedException();
	}

	public override void setTeams()
	{
		throw new System.NotImplementedException();
	}

	public override void update()
	{
		throw new System.NotImplementedException();
	}

	public override Player getWinner()
	{
		throw new System.NotImplementedException();
	}
}
