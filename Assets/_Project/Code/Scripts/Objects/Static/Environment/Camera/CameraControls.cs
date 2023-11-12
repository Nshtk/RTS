using UnityEngine;
using System;

public class CameraControls : MonoBehaviour
{
	public float speed_move=3f, speed_zoom=40f, speed_rotate=0.05f;
	private Vector3 _limits;
	private Vector3 _position_increment, _rotation_increment, _position_next;
	private Vector3 _mouse_position_1, _mouse_position_2;

	void Start()
	{
		_limits = new Vector3(TerrainGenerator.instance.length, TerrainGenerator.instance.width, TerrainGenerator.instance.height);
		transform.position = new Vector3(10, 15, 10);
	}
	void Update()
	{
		_position_increment=new Vector3(0, Input.GetAxis("Mouse ScrollWheel")*-speed_zoom*MathF.Log(transform.position.y+1), 0);
		{
			Vector3 _pos_temp=transform.forward;
			_pos_temp.y=0;
			//_pos_temp.Normalize();
			_position_increment+=transform.right*Input.GetAxis("Horizontal")*transform.position.y*speed_move
								+_pos_temp*Input.GetAxis("Vertical")*transform.position.y*speed_move;
		}
		_position_increment*=Time.deltaTime;

		if(Input.GetKey(KeyCode.LeftShift))
		{
			_position_increment.x*=3f;
			_position_increment.z*=3f;
		}
		_position_next=transform.position+_position_increment;
		if(_position_next.x<_limits.x && _position_next.x>0 && _position_next.y<_limits.y && _position_next.z<_limits.z&&_position_next.z>0 && _position_next.y>TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z]*_limits.y+5)
		{
			//if(_position_next.y<=TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z])
			//_position_next.y=Math.Min(TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z]+5, _limits.y);
			transform.position=_position_next;
		}

		if(Input.GetMouseButtonDown(1))
		{
			_mouse_position_1=Input.mousePosition;
		}
		if(Input.GetMouseButton(1))
		{
			_mouse_position_2=Input.mousePosition;
			_rotation_increment=(_mouse_position_2-_mouse_position_1)*speed_rotate;
			transform.rotation*=Quaternion.Euler(new Vector3(0, _rotation_increment.x, 0));
			transform.GetChild(0).transform.rotation*=Quaternion.Euler(new Vector3(-_rotation_increment.y, 0, 0));
			_mouse_position_1=_mouse_position_2;
		}
	}
}
