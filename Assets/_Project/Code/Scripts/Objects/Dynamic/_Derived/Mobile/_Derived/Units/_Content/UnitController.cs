using UnityEngine;

public partial class Unit : MobileObject
{
	public class UnitController : MobileObjectController
	{
		protected Unit _unit;

		public UnitController(Unit unit) : base(unit)
		{
			_unit=unit;
		}

		protected override void turn()
		{
			base.turn();
		}
		protected override void accelerate(Vector3 direction, float speed, float speed_max)	//TODO separate to accelerateForward()
		{
			if (!_unit.Is_Grounded)		//TODO To GroundUnit 
				return;
			_unit._rigid_body.AddForce(direction*speed*Game.instance.Terrain_Generator.map.tiles[(int)_unit.transform.position.x, (int)_unit.transform.position.z].traction);   //REVIEW * Time.fixedDeltaTime?
			if (_unit._rigid_body.velocity.magnitude > speed_max)
				_unit._rigid_body.velocity = _unit._rigid_body.velocity.normalized * speed_max;
		}
		protected override void move()
		{
			base.move();
		}
		public override void stop()
		{
			base.stop();
		}
		public override void moveToPosition(float remaining_distance_accuracy = 2f)
		{
			base.moveToPosition(remaining_distance_accuracy);
		}
		public void moveByPath(float remaining_distance_accuracy=2f)
		{
			for (int i = 1; i<_unit.navmesh_path.corners.Length; ++i)
				Debug.DrawLine(_unit.navmesh_path.corners[i-1], _unit.navmesh_path.corners[i], Color.green);
			if (_waypoint.remaining_distance<remaining_distance_accuracy)
			{
				if (_waypoint.id>=_unit.navmesh_path.corners.Length)
				{
					Is_Destination_Reached = true;
					return;
				}
				_waypoint.setPosition(_unit.navmesh_path.corners[_waypoint.id++], _unit.transform.position);
				//++_waypoint.id;
			}
			move();
		}
	}
}

