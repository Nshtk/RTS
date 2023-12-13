using UnityEngine;
using UnityEngine.AI;

public partial class Unit : MobileObject
{
	public abstract class UnitState
	{
		protected Unit _unit;
		public float weight_factor;
		protected UNIT_STATE unit_state;

		public UnitState(Unit unit, float weight_factor=1f)
		{
			_unit=unit;
			this.weight_factor=weight_factor;
		}

		public virtual void enter()
		{
			_unit.state=unit_state;
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
			unit_state=UNIT_STATE.IDLE;
		}
		public override void enter()
		{
			base.enter();
		}
		public override void update()
		{
			base.update();
			_unit._unit_controller.stop();
		}
	}
	public class UnitEvadeState : UnitState
	{
		public UnitEvadeState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.EVADING;
		}

		public override void enter()
		{
			base.enter();
			//bool path_found = false;

			if (_unit.destination!=null)
			{
				if (!NavMesh.CalculatePath(_unit.transform.position, _unit.destination.Value, _unit.navmesh_query_filter, _unit.navmesh_path))
					_unit.changeState(_unit.state_idle);
			}
			else
				_unit.changeState(_unit.state_idle);
		}
		public override void update()
		{
			base.update();

			if (_unit._unit_controller.Is_Destination_Reached)
			{
				_unit.changeState(_unit.state_idle);
				return;
			}
			_unit._unit_controller.moveByPath();
		}
		public override void exit()
		{
			_unit._unit_controller.Is_Destination_Reached=false;
		}
	}
	public class UnitFollowState : UnitState
	{
		public UnitFollowState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.FOLLOWING;
		}

		public override void enter()
		{
			base.enter();
			//bool path_found=false;

			if (_unit.target!=null)
			{
				if (!NavMesh.CalculatePath(_unit.transform.position, _unit.target.transform.position, _unit.navmesh_query_filter, _unit.navmesh_path))
					_unit.changeState(_unit.state_idle);
			}
			else
				_unit.changeState(_unit.state_idle);
		}
		public override void update()
		{
			if (_unit.target!=null)      //TODO NavMesh.SamplePosition() check if unit is outside of navmesh 
			{
				if (Game.instance.game_data.ticks%50==0 && Vector3.Distance(_unit.transform.position, _unit.target.transform.position)>5)
				{
					if(NavMesh.CalculatePath(_unit.transform.position, _unit.target.transform.position, _unit.navmesh_query_filter, _unit.navmesh_path))
					{
						_unit._unit_controller.Is_Destination_Reached=false;
					}
					else
					{
						_unit.changeState(_unit.state_idle);
						return;
					}
				}
			}
			_unit._unit_controller.moveByPath();
		}
		public override void exit()
		{
			_unit.target=null;
			_unit._unit_controller.Is_Destination_Reached=false;
		}
		/*private void CalculateTargetDestination()		//TODO follow until target bounds
		{
			//calculate number of unit vectors from unit centre to unit edge of bounds
			Vector3 originalExtents = selectionBounds.extents;
			Vector3 normalExtents = originalExtents;
			normalExtents.Normalize();
			float numberOfExtents = originalExtents.x / normalExtents.x;
			int unitShift = Mathf.FloorToInt(numberOfExtents);
		
			//calculate number of unit vectors from target centre to target edge of bounds
			WorldObject worldObject = destinationTarget.GetComponent<WorldObject>();
			if(worldObject)
				originalExtents = worldObject.GetSelectionBounds().extents;
			else
				originalExtents = new Vector3(0.0f, 0.0f, 0.0f);
			normalExtents = originalExtents;
			normalExtents.Normalize();
			numberOfExtents = originalExtents.x / normalExtents.x;
			int targetShift = Mathf.FloorToInt(numberOfExtents);
		
			//calculate number of unit vectors between unit centre and destination centre with bounds just touching
			int shiftAmount = targetShift + unitShift;
		
			//calculate direction unit needs to travel to reach destination in straight line and normalize to unit vector
			Vector3 origin = transform.position;
			Vector3 direction = new Vector3(destination.x - origin.x, 0.0f, destination.z - origin.z);
			direction.Normalize();
		
			//destination = center of destination - number of unit vectors calculated above
			//this should give us a destination where the unit will not quite collide with the target
			//giving the illusion of moving to the edge of the target and then stopping
			for(int i = 0; i<shiftAmount; i++)
				destination -= direction;
			destination.y = destinationTarget.transform.position.y;
		}*/
	}
	public class UnitPatrolState : UnitState
	{
		float timeBeforeSleep;

		public UnitPatrolState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.PATROLING;
		}

		public override void enter()
		{
			timeBeforeSleep = 20;
		}
		public override void update()
		{
			if (Physics.Raycast(_unit.transform.position, _unit.transform.forward))
				_unit.changeState(_unit.state_follow);
			if (timeBeforeSleep < 0)
				_unit.changeState(_unit.state_idle);
			timeBeforeSleep -= Time.deltaTime;
		}
		public override void exit()
		{
		}
	}
	public class UnitEngageState : UnitState
	{
		public UnitEngageState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.ENGAGING;
		}
	}
}
