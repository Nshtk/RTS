using UnityEngine;

public partial class GroundUnit : Unit
{
    public float traction;
	private GroundUnitIdleState state_idle;
	private GroundUnitFollowState state_follow;

	protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
		state_current?.update();	//REVIEW TEMP, move to updateManual
	}

    public override void setStates()
    {
		state_idle=new GroundUnitIdleState(this);
		state_follow=new GroundUnitFollowState(this);
		changeState(state_idle);
	}
	public override void setOrder(Vector3 position, DynamicObject target = null)
	{
		destination=position;
		if (target!=null)
		{
			this.target=target;
			changeState(state_follow);
		}
		else
			changeState(state_idle);
	}
}