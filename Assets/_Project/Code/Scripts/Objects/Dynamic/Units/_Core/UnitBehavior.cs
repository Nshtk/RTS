using UnityEngine;

public partial class UnitBehavior : MonoBehaviour
{
	public SleepState sleepState = new SleepState();
	public ChaseState chaseState = new ChaseState();
	public PatrolState patrolState = new PatrolState();
	public HurtState hurtState = new HurtState();

	private void Start()
    {
		ChangeState(patrolState);
	}
    private void Update()
    {
		if(currentState != null)
		{
			currentState.UpdateState(this);
		}
	}

	public void ChangeState(IUnitBehaviorState newState)
	{
		if(currentState != null)
		{
			currentState.OnExit(this);
		}
		currentState = newState;
		currentState.OnEnter(this);
	}
}
