using UnityEngine;

public partial class GroundUnit : Unit
{
	protected UnitPatrolState state_patrol;
	protected UnitEngageState state_engage;

	public float grip;	//TODO add to move calculation

	public override string Name
	{
		get { return "VehicleGround"; }
	}

	protected override void Awake()
	{
		base.Awake();
	}
	protected override void Start()
    {
        base.Start();
	}
    protected override void Update()
    {
        base.Update();
		state_current?.update();	//REVIEW move to updateManual
	}

    public override void setStates()
    {
		state_idle=     new UnitIdleState(this);//TODO new GroundUnitIdleState(this);
		state_evade=    new UnitEvadeState(this);
		state_follow=   new UnitFollowState(this);
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