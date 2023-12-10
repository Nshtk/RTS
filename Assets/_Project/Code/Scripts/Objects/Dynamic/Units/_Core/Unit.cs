using System;
using UnityEngine;
using UnityEngine.AI;

public partial class Unit : DynamicObject
{
	public enum UNIT_STATE
	{
		IDLE,
		FOLLOWING,
		EVADING,
		PATROLING,
		ENGAGING
	}
	public enum UNIT_TYPE
	{
		INFANTRY,
		INFANTRY_AT,
		VEHICLE,
		TANK,
		ARTILLERY,
		AIRPLANE_FIGHTER,
		AIRPLANE_BOMBER,
		SPECIAL
	}
	public enum UNIT_MOVEMENT_TYPE
	{
		TRACKED,
		FOOTED,
		WHEELED,

	}

	public class UnitController
	{
		private struct Waypoint
		{
			public int id;
			public Vector3 position;
			public float remaining_distance;

			public Waypoint(Vector3? position = null, int id=0) 
			{
				this.position = position.Value;
				this.id=id;
				remaining_distance = -1f;
			}

			public void setPosition(Vector3 waypoint, Vector3 position_current)
			{
				position = waypoint;
				updateRemainingDistance(position_current);
			}
			public void updateRemainingDistance(Vector3 position_current)
			{
				remaining_distance=Vector3.Distance(position_current, position);
			}
		}

		private Unit _unit;
		public float speed_move = 10f, speed_move_max = 15f;    //TODO to ctor
		public float speed_rotate = 15f, speed_rotate_max = 45f;
		public float acceleration = 10f, acceleration_angular = 45f;
		public float orientation;
		public Vector3 velocity = Vector3.one;
		public Vector3 direction;
		private Waypoint _waypoint;
		private bool _is_destination_reached;
		private Quaternion rotation_direction;

		public bool Is_Destination_Reached
		{
			get { return _is_destination_reached; }
			set
			{
				if(!value)
				{
					_waypoint.id=0;
					_waypoint.remaining_distance=-1f;
				}
				_is_destination_reached=value;
			}
		}

		public UnitController(Unit unit)
		{
			_unit=unit;
		}

		private void turn(Quaternion direction)		//REVIEW
		{
			_unit.transform.rotation = Quaternion.RotateTowards(_unit.transform.rotation, direction, speed_rotate_max*Time.deltaTime);
		}
		public virtual void move()
		{
			direction = (_waypoint.position-_unit.transform.position).normalized;
			rotation_direction= Quaternion.FromToRotation(Vector3.forward, direction);

			turn(rotation_direction);
			if (_unit.movement_type==UNIT_MOVEMENT_TYPE.TRACKED && Quaternion.Angle(_unit.transform.rotation, rotation_direction)>90)
			{
				return;
			}
			//_unit.transform.rotation = Quaternion.LookRotation(direction);

			_unit._rigid_body.AddForce(direction.normalized*speed_move);   //REVIEW * Time.fixedDeltaTime?
			if (_unit._rigid_body.velocity.magnitude > speed_move_max)
				_unit._rigid_body.velocity = _unit._rigid_body.velocity.normalized * speed_move_max;
			_waypoint.updateRemainingDistance(_unit.transform.position);
		}
		public void moveToPosition(float remaining_distance_accuracy = 2f)
		{
			if (_waypoint.remaining_distance<remaining_distance_accuracy)
			{
				Is_Destination_Reached = true;
				return;
			}
			move();
		}
		public void moveByPath(float remaining_distance_accuracy=2f)
		{
			if(_waypoint.remaining_distance<remaining_distance_accuracy)
			{
				if (_waypoint.id>=_unit.navmesh_path.corners.Length)
				{
					Is_Destination_Reached = true;
					return;
				}
				_waypoint.setPosition(_unit.navmesh_path.corners[_waypoint.id], _unit.transform.position);
				++_waypoint.id;
			}
			move();
		}
		public void stop()
		{

		}
	}

	public AudioClip sound_voiceover, sound_idle, sound_move;
	public Player _player_owner;
	UNIT_STATE state;
	UNIT_TYPE type;
	UNIT_MOVEMENT_TYPE movement_type;
	public int limit;
	public int cost;
	public int charge_time, recharge_time;
	public float experience;

	protected UnitController _unit_controller;	//TODO to private
	protected UnitState state_current;
	protected UnitIdleState state_idle;
	protected UnitFollowState state_follow;
	protected UnitEvadeState state_evade;

	protected Rigidbody _rigid_body;
	protected NavMeshPath navmesh_path;
	protected NavMeshQueryFilter navmesh_query_filter;
	public string[] layerNames; //REVIEW

	public virtual string Name
	{
		get { return "Unit"; }
	}

	protected override void Awake()
	{
		base.Awake();
		_rigid_body = GetComponent<Rigidbody>();
		navmesh_query_filter=new NavMeshQueryFilter() { agentTypeID=GetNavMeshAgentID(), areaMask= 1<<0 | 1<<3};
		navmesh_path =new NavMeshPath();
		_unit_controller=new UnitController(this);
	}
	protected override void Start()
    {
        base.Start();
		unitDied+=Game.GameData.instance.handleUnitDied;
		setStates();
	}
	public override void StartManual()
	{

	}
	protected override void Update()
    {
        base.Update();
    }
	public override void UpdateManual()
	{
		state_current?.update();
	}
	protected override void FixedUpdate()
	{
		
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		unitDied?.Invoke(this, new UnitDiedEventArgs("unit died"));
	}

	public override void initialise(Player owner)
	{
		_player_owner=owner;
	}
	protected int GetNavMeshAgentID()
	{
		for(int i=0; i<NavMesh.GetSettingsCount(); i++)
		{
			NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(index: i);
			if (Name == NavMesh.GetSettingsNameFromID(agentTypeID: settings.agentTypeID))
				return settings.agentTypeID;
		}
		return 1;
	}
	public void setChargeTimer()
	{

	}
	public virtual void setStates()
	{
		state_idle=     new UnitIdleState(this);
		state_follow=   new UnitFollowState(this);
		state_evade=    new UnitEvadeState(this);
		changeState(state_idle);
	}

	public virtual void setOrder(Vector3 position, DynamicObject target=null)
	{
		destination=position;
		this.target=target;
		if (target!=null)
			changeState(state_follow);
		else
			changeState(state_idle);
	}
	public void changeState(UnitState state_next)
	{
		state_current?.exit();
		state_current = state_next;
		state_current.enter();
	}
	/*public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		base.MouseClick(hitObject, hitPoint, controller);
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected)
		{
			bool clickedOnEmptyResource = false;
			if(hitObject.transform.parent)
			{
				Resource resource = hitObject.transform.parent.GetComponent<Resource>();
				if(resource && resource.isEmpty())
					clickedOnEmptyResource = true;
			}
			if((WorkManager.ObjectIsGround(hitObject) || clickedOnEmptyResource) && hitPoint != ResourceManager.InvalidPosition)
			{
				float x = hitPoint.x;
				//makes sure that the unit stays on top of the surface it is on
				float y = hitPoint.y + player.SelectedObject.transform.position.y;
				float z = hitPoint.z;
				Vector3 destination = new Vector3(x, y, z);
				StartMove(destination);
			}
		}
	}*/

	/*public virtual void StartMove(Vector3 destination)
	{
		if(audioElement != null)
			audioElement.Play(moveSound);
		this.destination = destination;
		destinationTarget = null;
		targetRotation = Quaternion.LookRotation(destination - transform.position);
		rotating = true;
		moving = false;
		attacking = false;
	}*/

	/*public void StartMove(Vector3 destination, GameObject destinationTarget)
	{
		StartMove(destination);
		this.destinationTarget = destinationTarget;
	}*/

	/*public override void SaveDetails(JsonWriter writer)
	{
		base.SaveDetails(writer);
		SaveManager.WriteBoolean(writer, "Moving", moving);
		SaveManager.WriteBoolean(writer, "Rotating", rotating);
		SaveManager.WriteVector(writer, "Destination", destination);
		SaveManager.WriteQuaternion(writer, "TargetRotation", targetRotation);
		if(destinationTarget)
		{
			WorldObject destinationObject = destinationTarget.GetComponent<WorldObject>();
			if(destinationObject)
				SaveManager.WriteInt(writer, "DestinationTargetId", destinationObject.ObjectId);
		}
	}*/

	/*protected override void HandleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		base.HandleLoadedProperty(reader, propertyName, readValue);
		switch(propertyName)
		{
			case "Moving":
				moving = (bool)readValue;
				break;
			case "Rotating":
				rotating = (bool)readValue;
				break;
			case "Destination":
				destination = LoadManager.LoadVector(reader);
				break;
			case "TargetRotation":
				targetRotation = LoadManager.LoadQuaternion(reader);
				break;
			case "DestinationTargetId":
				loadedDestinationTargetId = (int)(System.Int64)readValue;
				break;
			default:
				break;
		}
	}*/

	/*protected override bool ShouldMakeDecision()
	{
		if(moving || rotating)
			return false;
		return base.ShouldMakeDecision();
	}*/

	/*private void CalculateTargetDestination()
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

	/*private void MakeMove()
	{
		transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
		if(transform.position == destination)
		{
			moving = false;
			movingIntoPosition = false;
			if(audioElement != null)
				audioElement.Stop(driveSound);
		}
		CalculateBounds();
	}*/
	public delegate void unitDiedEventHandler(Unit sender, UnitDiedEventArgs e);
	public static event unitDiedEventHandler unitDied;
	public class UnitDiedEventArgs : EventArgs
	{
		public string Message
		{
			get;
			set;
		}
		public UnitDiedEventArgs(string message)
		{
			Message = message;
		}
	}
}
