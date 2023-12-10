using UnityEngine;
using UnityEngine.AI;

public partial class Unit : DynamicObject
{
	public abstract class UnitState
	{
		protected Unit _unit;
		public float angular;
		public Vector3 linear;
		public float weight;

		public UnitState(Unit unit)
		{
			_unit=unit;
		}

		public virtual void enter()
		{

		}
		public virtual void update()
		{

		}
		public virtual void onHurt()
		{

		}
		public virtual void exit()
		{
		}
	}

	public class UnitIdleState : UnitState
	{
		public UnitIdleState(Unit unit) : base(unit)
		{

		}

		public override void update()
		{
			base.update();
			_unit._unit_controller.stop();
			if (_unit.destination!=null)
			{
				_unit.changeState(_unit.state_follow);
			}
		}
	}
	public class UnitFollowState : UnitState
	{
		private float time_elapsed_since_path_update;

		public UnitFollowState(Unit unit) : base(unit)
		{
		}
		public override void enter()
		{
			bool path_found;

			if (_unit.target==null)
				path_found=NavMesh.CalculatePath(_unit.transform.position, _unit.destination.Value, _unit.navmesh_query_filter, _unit.navmesh_path);
			else
				path_found=NavMesh.CalculatePath(_unit.transform.position, _unit.target.transform.position, _unit.navmesh_query_filter, _unit.navmesh_path);
			if (path_found)
			{
				time_elapsed_since_path_update=0f;
				_unit._unit_controller.Is_Destination_Reached=false;
			}
			else
				_unit.changeState(_unit.state_idle);
		}
		public override void update()
		{
			for (int i = 1; i<_unit.navmesh_path.corners.Length; ++i)
				Debug.DrawLine(_unit.navmesh_path.corners[i-1], _unit.navmesh_path.corners[i], Color.green);
			if (_unit.target!=null)      //TODO NavMesh.SamplePosition() check if unit is outside of navmesh 
			{
				time_elapsed_since_path_update += Time.deltaTime;
				if (time_elapsed_since_path_update>1f && Vector3.Distance(_unit.transform.position, _unit.target.transform.position)>5)
				{
					if (!NavMesh.CalculatePath(_unit.transform.position, _unit.target.transform.position, _unit.navmesh_query_filter, _unit.navmesh_path))
					{
						_unit.changeState(_unit.state_idle);
						return;
					}
				}
			}
			else
			{
				if (_unit._unit_controller.Is_Destination_Reached)//_unit.navmesh_agent.remainingDistance<2)
				{
					_unit.changeState(_unit.state_idle);
					return;
				}
			}
			_unit._unit_controller.moveByPath();
		}
		public override void exit()
		{
			_unit.target=null;
			_unit.destination=null;
			_unit._unit_controller.Is_Destination_Reached=false;
		}
	}
	public class UnitEvadeState : UnitState
	{
		public UnitEvadeState(Unit unit) : base(unit)
		{

		}
	}
	public class UnitPatrolState : UnitState
	{
		float timeBeforeSleep;

		public UnitPatrolState(Unit unit) : base(unit)
		{
		}

		public void OnEnter()
		{
			timeBeforeSleep = 20;
		}
		public void UpdateState()
		{
			if (Physics.Raycast(_unit.transform.position, _unit.transform.forward))
			{
				//_unit.ChangeState(_unit.chaseState);
			}
			if (timeBeforeSleep < 0)
			{
				//_unit.ChangeState(_unit.sleepState);
			}
			timeBeforeSleep -= Time.deltaTime;
		}
		public void OnHurt()
		{
			//_unit.ChangeState(_unit.hurtState);
		}
		public void OnExit()
		{
		}
	}
	public class UnitEngageState : UnitState
	{
		public UnitEngageState(Unit unit) : base(unit)
		{

		}
	}
}
