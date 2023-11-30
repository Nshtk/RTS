using UnityEngine;

public partial class UnitBehavior
{
	public abstract class UnitBehaviorState
	{
		protected UnitBehavior _unit_behavior;
		public float angular;
		public Vector3 linear;
		public float weight;

		public UnitBehaviorState(UnitBehavior unit_behavior)
		{
			_unit_behavior=unit_behavior;
		}

		public virtual void enter()
		{

		}
		public void update()
		{

		}
		public virtual void exit()
		{
		}
	}
	public class IdleState : UnitBehaviorState
	{
		public IdleState(UnitBehavior unit_behavior) : base(unit_behavior)
		{

		}
	}
	public class FollowState : UnitBehaviorState
	{
		public DynamicObject _dynamic_object_target;

		public FollowState(UnitBehavior unit_behavior) : base(unit_behavior)
		{
			//unit_behavior.navmesh_agent.destination=;
		}
	}
	public class EvadeState : UnitBehaviorState
	{
		public EvadeState(UnitBehavior unit_behavior) : base(unit_behavior)
		{

		}
	}
	public class PatrolState : UnitBehaviorState
	{
		float timeBeforeSleep;

		public PatrolState(UnitBehavior unit_behavior) : base(unit_behavior)
		{
		}

		public void OnEnter()
		{
			timeBeforeSleep = 20;
		}
		public void UpdateState()
		{
			if(Physics.Raycast(_unit_behavior.transform.position, _unit_behavior.transform.forward))
			{
				//_unit_behavior.ChangeState(_unit_behavior.chaseState);
			}
			if(timeBeforeSleep < 0)
			{
				//_unit_behavior.ChangeState(_unit_behavior.sleepState);
			}
			timeBeforeSleep -= Time.deltaTime;
		}
		public void OnHurt()
		{
			//_unit_behavior.ChangeState(_unit_behavior.hurtState);
		}
		public void OnExit()
		{
		}
	}
	public class EngageState : UnitBehaviorState
	{
		public EngageState(UnitBehavior unit_behavior) : base(unit_behavior)
		{

		}
	}
}

