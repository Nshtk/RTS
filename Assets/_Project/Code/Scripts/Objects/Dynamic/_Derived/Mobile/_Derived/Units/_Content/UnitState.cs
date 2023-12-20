using UnityEngine;
using UnityEngine.AI;

public partial class Unit : MobileObject
{
	public abstract class UnitState
	{
		protected Unit _unit;
		public float weight_factor;
		protected UNIT_STATE unit_state;
		protected UNIT_STATUS unit_status;

		public UnitState(Unit unit, float weight_factor=1f)
		{
			_unit=unit;
			this.weight_factor=weight_factor;
		}

		public virtual void enter()
		{
			_unit.state=unit_state;
			_unit.status=unit_status;
		}
		public virtual void update()	//TODO to abstract?
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
			unit_status=UNIT_STATUS.WAITING;
		}

		public override void enter()
		{
			base.enter();
		}
		public override void update()
		{
			base.update();
			_unit._audio_source.PlayOneShot(_unit.sound_idle);
			_unit._unit_controller.stop();
		}
	}
	public class UnitEvadeState : UnitState
	{
		public UnitEvadeState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.EVADING;
			unit_status=UNIT_STATUS.MOVING;
		}

		public override void enter()
		{
			base.enter();

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
		private Collider target_collider;

		public UnitFollowState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.FOLLOWING;
			unit_status=UNIT_STATUS.MOVING;
		}

		public override void enter()
		{
			base.enter();

			if (_unit.target!=null)
			{
				target_collider = _unit.target.GetComponent<Collider>();
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
				if (Game.instance.game_data.ticks%50==0 && Vector3.Distance(_unit.transform.position, target_collider.bounds.extents)>5)
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
			else
			{
				_unit.changeState(_unit.state_idle);
				return;
			}
			_unit._unit_controller.moveByPath();
		}
		public override void exit()
		{
			_unit.target=null;
			_unit._unit_controller.Is_Destination_Reached=false;
		}
	}
	public class UnitPatrolState : UnitState
	{
		float timeBeforeSleep;

		public UnitPatrolState(Unit unit) : base(unit)
		{
			unit_state=UNIT_STATE.PATROLING;
			unit_status=UNIT_STATUS.MOVING;
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
