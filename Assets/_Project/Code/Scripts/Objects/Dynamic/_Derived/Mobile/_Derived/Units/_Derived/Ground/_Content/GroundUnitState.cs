using UnityEngine;
using UnityEngine.AI;

public partial class GroundUnit : Unit
{
	public abstract class GroundUnitState : UnitState
	{
		protected GroundUnit _unit_ground;

		public GroundUnitState(GroundUnit unit) : base(unit)
		{
			_unit_ground = unit;
		}

		public override void enter()
		{

		}
		public override void update()
		{

		}
		public override void exit()
		{
		}
	}

	public class GroundUnitIdleState : GroundUnitState
	{
		public GroundUnitIdleState(GroundUnit unit) : base(unit)
		{

		}

		public override void update()
		{
			base.update();
			if (_unit.destination!=null)
			{
				_unit.changeState(_unit_ground.state_follow);
			}
		}
	}
	public class GroundUnitFollowState : GroundUnitState
	{
		private float time_elapsed_since_path_update;

		public GroundUnitFollowState(GroundUnit unit) : base(unit)
		{
		}
		public override void enter()
		{
			bool path_found;

			if (_unit_ground.target==null)
				path_found=NavMesh.CalculatePath(_unit_ground.transform.position, _unit_ground.destination.Value, _unit_ground.navmesh_query_filter, _unit_ground.navmesh_path);
			else 
				path_found=NavMesh.CalculatePath(_unit_ground.transform.position, _unit_ground.target.transform.position, _unit_ground.navmesh_query_filter, _unit_ground.navmesh_path);
			if (path_found)
			{
				time_elapsed_since_path_update=0f;
				_unit_ground._unit_controller.Is_Destination_Reached=false;
			}
			else
				_unit.changeState(_unit_ground.state_idle);
		}
		public override void update()
		{
			for (int i = 1; i<_unit_ground.navmesh_path.corners.Length; ++i)
				Debug.DrawLine(_unit_ground.navmesh_path.corners[i-1], _unit_ground.navmesh_path.corners[i], Color.green);
			if (_unit_ground.target!=null)		//TODO NavMesh.SamplePosition() check if unit is outside of navmesh 
			{
				time_elapsed_since_path_update += Time.deltaTime;
				if (time_elapsed_since_path_update>1f && Vector3.Distance(_unit_ground.transform.position, _unit_ground.target.transform.position)>5)
				{
					if(!NavMesh.CalculatePath(_unit_ground.transform.position, _unit_ground.target.transform.position, _unit_ground.navmesh_query_filter, _unit_ground.navmesh_path))
					{
						_unit_ground.changeState(_unit_ground.state_idle);
						return;
					}
				}
			}
			else
			{
				if (_unit_ground._unit_controller.Is_Destination_Reached)//_unit_ground.navmesh_agent.remainingDistance<2)
				{
					_unit_ground.changeState(_unit_ground.state_idle);
					return;
				}
			}
			_unit_ground._unit_controller.moveByPath();
		}
		public override void exit()
		{
			_unit_ground.target=null;
			_unit_ground.destination=null;
			_unit_ground._unit_controller.Is_Destination_Reached=false;
		}
	}
	public class GroundUnitPatrolState : GroundUnitState
	{
		private Vector3[] _positions;
		private int position_next;
		float time_before_sleep;
		float distance;

		public GroundUnitPatrolState(GroundUnit unit, Vector3[] positions) : base(unit)
		{
			_positions =positions;
			position_next=0;
		}

		public override void enter()
		{
			time_before_sleep = 20;
		}
		public override void update()
		{
			/*if (_unit_ground.navmesh_agent.remainingDistance < distance)
			{
				position_next++;
				if (position_next == _positions.Length)
					position_next = 0;
				_unit_ground.navmesh_agent.destination = _positions[position_next];
			}*/
		}
		public override void exit()
		{
		}
	}
	public class GroundUnitEngageState : GroundUnitState
	{
		public GroundUnitEngageState(GroundUnit unit) : base(unit)
		{

		}
	}
}

