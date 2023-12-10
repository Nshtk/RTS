using UnityEngine;

public partial class AirUnit : Unit
{
	protected UnitPatrolState state_patrol;
	protected UnitEngageState state_engage;

	public override string Name
	{
		get { return "VehicleAir"; }
	}

	protected override void Start()
	{
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
		state_current?.update();
	}

	public override void setStates()
	{
		state_idle=     new UnitIdleState(this);//new AirUnitIdleState(this);
		state_follow=   new UnitFollowState(this);//new AirUnitFollowState(this);
		state_patrol=   new UnitPatrolState(this);
		state_engage=   new UnitEngageState(this);
		changeState(state_idle);
	}

	/*public override void setOrder(Vector3 position, DynamicObject target = null)
	{
		destination=position;
		if (target!=null)
		{
			this.target=target;
			changeState(state_follow);
		}
		else
			changeState(state_idle);
	}*/
}
