using UnityEngine;
using UnityEngine.AI;

public partial class UnitBehavior : MonoBehaviour
{
	public enum UNIT_BEHAVIOR_STATE
	{
		IDLE,
		FOLLOWING,
		EVADING,
		PATROLING,
		ENGAGING
	}

	public UnitBehaviorState state_current;
	public IdleState   state_idle;
	public FollowState state_follow;
	public EvadeState  state_evade;
	public PatrolState state_patrol;
	public EngageState state_engage;

	public float speed_move=1f, speed_rotate=45f;
	public float acceleration = 30f, acceleration_angular = 45f;
	public float orientation;
	public Vector3 velocity=Vector3.one;

	NavMeshAgent navmesh_agent;

	private void Awake()
	{
		state_idle=		new	IdleState(this);
		state_follow=	new	FollowState(this);
		state_evade=	new	EvadeState(this);
		state_patrol=	new	PatrolState(this);
		state_engage=	new EngageState(this);
		navmesh_agent = GetComponent<NavMeshAgent>();
	}
	private void Start()
    {
		changeState(state_patrol);
	}
    private void Update()
    {
		state_current?.update();
		Vector3 displacement = velocity * Time.deltaTime;
		displacement.y = 0;

		orientation += speed_rotate * Time.deltaTime;

		//limit orientation between 0 and 360
		if(orientation < 0.0f)
		{
			orientation += 360.0f;
		}
		else if(orientation > 360.0f)
		{
			orientation -= 360.0f;
		}
		transform.Translate(displacement, Space.World);
		transform.rotation = new Quaternion();
		transform.Rotate(Vector3.up, orientation);
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
	public void changeState(UnitBehaviorState state_next)
	{
		state_current?.exit();
		state_current = state_next;
		state_current.enter();
	}
}
