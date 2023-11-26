using UnityEngine;

public partial class UnitBehavior
{
	public abstract class UnitBehaviorState
	{
		protected UnitBehavior unit_behavior;

		public UnitBehaviorState(UnitBehavior unit_behavior)
		{
			this.unit_behavior=unit_behavior;
		}

		public virtual void OnEnter()
		{
		}
		public virtual void OnUpdate()
		{
		}
		public virtual void OnHurt()
		{
		}
		public virtual void OnExit()
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
			if(Physics.Raycast(unit_behavior.transform.position, unit_behavior.transform.forward))
			{
				unit_behavior.ChangeState(unit_behavior.chaseState);
			}
			if(timeBeforeSleep < 0)
			{
				unit_behavior.ChangeState(unit_behavior.sleepState);
			}
			timeBeforeSleep -= Time.deltaTime;
		}
		public void OnHurt()
		{
			unit_behavior.ChangeState(unit_behavior.hurtState);
		}
		public void OnExit()
		{
		}
	}
}

