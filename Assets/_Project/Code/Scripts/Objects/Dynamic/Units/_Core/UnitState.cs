using UnityEngine;

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
			if(_unit.destination!=null)
			{
				_unit.changeState(_unit.state_follow);
			}
		}
	}
	public class UnitFollowState : UnitState
	{
		public DynamicObject _dynamic_object_target;

		public UnitFollowState(Unit unit) : base(unit)
		{
			
		}
		public override void enter()
		{
			_unit.navmesh_agent.destination=_unit.destination.Value;
		}
		public override void update()
		{
			Vector3 displacement = _unit.velocity * Time.deltaTime;
			displacement.y = 0;
			_unit.transform.rotation = new Quaternion();

			_unit.orientation += _unit.speed_rotate * Time.deltaTime;
			if(_unit.orientation < 0.0f)
			{
				_unit.orientation += 360.0f;
			}
			else if(_unit.orientation > 360.0f)
			{
				_unit.orientation -= 360.0f;
			}
			_unit.transform.Translate(displacement, Space.World);
			_unit.transform.Rotate(Vector3.up, _unit.orientation);
		}
		public override void exit()
		{

		}
		/*public virtual void LateUpdate()
	{
		velocity += steer.linear * Time.deltaTime;
		speed_rotate += steer.angular * Time.deltaTime;
		if(velocity.magnitude > maxSpeed)
		{
			velocity.Normalize();
			velocity = velocity * maxSpeed;
		}

		if(steer.linear.magnitude == 0.0f)
		{
			velocity = Vector3.zero;
		}
		steer = new steering();
	}*/

		/*public float MapToRange(float rotation)
		{
			rotation %= 360.0f;
			if(Mathf.Abs(rotation) > 180.0f)
			{
				if(rotation < 0.0f)
				{
					rotation += 360.0f;
				}
				else
				{
					rotation -= 360.0f;
				}
			}

			return rotation;
		}*/
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
