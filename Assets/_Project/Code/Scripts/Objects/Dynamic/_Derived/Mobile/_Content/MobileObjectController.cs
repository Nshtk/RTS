using UnityEngine;

public partial class MobileObject
{
	public class MobileObjectController
	{
		protected struct Waypoint
		{
			public int id;
			public Vector3 position;
			public float remaining_distance;

			public Waypoint(Vector3? position = null, int id = 0)
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

		protected MobileObject _mobile_object;
		public float speed_move = 10f, speed_move_max = 15f;    //TODO to ctor
		public float speed_stop = 5, speed_stop_max = 10;
		public float speed_rotate = 15f, speed_rotate_max = 45f;
		public float acceleration = 10f, acceleration_angular = 45f;
		public float orientation;
		public Vector3 direction;
		protected Waypoint _waypoint;
		protected bool _is_destination_reached;
		protected Quaternion rotation_direction;

		public bool Is_Destination_Reached
		{
			get { return _is_destination_reached; }
			set
			{
				if (!value)
				{
					_waypoint.id=0;
					_waypoint.remaining_distance=-1f;
					_mobile_object.destination=null;
				}
				_is_destination_reached=value;
			}
		}

		public MobileObjectController(MobileObject mobile_object)
		{
			_mobile_object=mobile_object;
		}

		protected virtual void turn()		//FIXME all methods
		{
			_mobile_object.transform.rotation=Quaternion.RotateTowards(_mobile_object.transform.rotation, rotation_direction, speed_rotate_max*Time.deltaTime);
		}
		protected virtual void accelerate(Vector3 direction, float speed, float speed_max) //TODO separate to accelerateForward()
		{

		}
		protected virtual void move()
		{
			direction = (_waypoint.position-_mobile_object.transform.position).normalized;
			rotation_direction= Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));//Quaternion.FromToRotation(_unit._rigid_body.rotation*Vector3.forward, direction);

			Debug.DrawRay(_mobile_object.transform.position, _mobile_object.transform.forward*3, Color.blue, 0f, true);

			if (Vector3.Dot(_mobile_object.transform.up, Vector3.down)>0)  //Reset flipped object
			{
				_mobile_object.transform.up=Vector3.up;
				_mobile_object.transform.forward=direction;
			}
			if (Vector3.Angle(new Vector3(direction.x, 0, direction.z), new Vector3(_mobile_object.transform.forward.x, 0, _mobile_object.transform.forward.z))>10)        //(Quaternion.Angle(Quaternion.LookRotation(new Vector3(_unit.transform.rotation.x, 0, _unit.transform.rotation.z)), rotation_direction)>0
			{
				turn();
				if (_mobile_object.movement_type==MOBILE_OBJECT_MOVEMENT_TYPE.TRACKED)
					return;
			}

			accelerate(_mobile_object.transform.forward, speed_move, speed_move_max);//
			_waypoint.updateRemainingDistance(_mobile_object.transform.position);
		}
		public virtual void stop()
		{
			if ((_mobile_object.transform.forward * Vector3.Dot(_mobile_object.transform.forward, _mobile_object._rigidbody.velocity)).magnitude>0)
				accelerate(-_mobile_object.transform.forward, speed_stop, speed_stop_max);
		}
		public virtual void moveToPosition(float remaining_distance_accuracy = 2f)
		{
			if (_waypoint.remaining_distance<remaining_distance_accuracy)
			{
				Is_Destination_Reached = true;
				return;
			}
			move();
		}

		/*if(audioElement != null)
			audioElement.Play(moveSound);*/
	}
}

