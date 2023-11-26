using System;
using UnityEngine;

public class Unit : DynamicObject
{
	public enum UNIT_CLASS
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
	public enum UNIT_STATUS
	{
		IDLE,
		FOLLOWING,
		EVADING,
		PATROLING,
		ENGAGING
	}

	public float acceleration;
    public float move_speed, rotate_speed;
    public AudioClip sound_voiceover, sound_idle, sound_move;
	private Player _player_owner;
	private Vector3 destination;
	private Quaternion target_rotation;
	private GameObject destination_target;
	//private int loadedDestinationTargetId = -1;
	public int limit;
	public int cost;
	public int charge_time, recharge_time;
	public float experience;

	protected override void Start()
    {
        base.Start();
		unitDied+=Game.GameData.instance.handleUnitDied;
	}
	protected override void Update()
    {
        base.Update();
    }
	public void setChargeTimer()
	{
	
	}
	public override void initialise(Player owner)
	{
		_player_owner=owner;
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		unitDied?.Invoke(this, new UnitDiedEventArgs("unit died"));
	}

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

	/*private void TurnToTarget()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
		CalculateBounds();
		//sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
		Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
		{
			rotating = false;
			moving = true;
			if(destinationTarget)
				CalculateTargetDestination();
			if(audioElement != null)
				audioElement.Play(driveSound);
		}
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
}
