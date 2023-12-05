using UnityEngine;

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
		public GroundUnitFollowState(GroundUnit unit) : base(unit)
		{

		}
		public override void enter()
		{
			if (_unit_ground.target==null)
				_unit_ground.navmesh_agent.SetDestination(_unit_ground.destination.Value);
		}
		public override void update()
		{
			if (_unit_ground.navmesh_agent.remainingDistance>2)
			{
				if (_unit_ground.target!=null)
					_unit_ground.navmesh_agent.SetDestination(_unit_ground.target.transform.position);
			}
			else
				_unit.changeState(_unit_ground.state_idle);

			/*var heading = target.Value.transform.position - transform.position;
			var distance = heading.magnitude;
			var direction = heading / distance;


			Vector3 movement = direction * Time.deltaTime * speed.Value;
			agent.Move(movement);*/

			/*Vector3 displacement = _unit_ground.velocity * Time.deltaTime;
			displacement.y = 0;
			_unit_ground.transform.rotation = new Quaternion();

			_unit_ground.orientation += _unit_ground.speed_rotate * Time.deltaTime;
			if (_unit_ground.orientation < 0.0f)
			{
				_unit_ground.orientation += 360.0f;
			}
			else if (_unit_ground.orientation > 360.0f)
			{
				_unit_ground.orientation -= 360.0f;
			}
			_unit_ground.transform.Translate(displacement, Space.World);
			_unit_ground.transform.Rotate(Vector3.up, _unit.orientation);*/
		}
		public override void exit()
		{

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
			if (_unit_ground.navmesh_agent.remainingDistance < distance)
			{
				position_next++;
				if (position_next == _positions.Length)
					position_next = 0;
				_unit_ground.navmesh_agent.destination = _positions[position_next];
			}
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

