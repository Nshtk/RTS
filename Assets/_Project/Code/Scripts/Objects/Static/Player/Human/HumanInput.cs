using System;
using UnityEngine;

public partial class Human
{
	public class HumanInput
	{
		private struct MouseBehavior
		{
			public float time_rmb_pressed;
			public float duration_rmb_held;
			public bool is_rmb_held;

			public MouseBehavior(float time_rmb_pressed, float duration_rmb_held, bool is_rmb_held)
			{
				this.time_rmb_pressed=time_rmb_pressed;
				this.duration_rmb_held=duration_rmb_held;
				this.is_rmb_held=is_rmb_held;
			}
		}

		private Human _human;
		private Transform transform_camera;     //TODO: feature split screen between multiple cameras
		public float speed_move = 3f, speed_zoom = 40f, speed_rotate = 0.05f;
		private Vector3 _limits;
		private Vector3 _position_increment, _rotation_increment, _position_next;
		private Vector3 _mouse_position_1, _mouse_position_2;
		private MouseBehavior _mouse_behavior=new MouseBehavior(0f, 0.25f, false);

		public HumanInput(Human human)
		{
			_human=human;
			transform_camera=Camera.main.GetComponentsInParent<Transform>()[1];
			_limits = new Vector3(TerrainGenerator.instance.length, TerrainGenerator.instance.width, TerrainGenerator.instance.height);
			transform_camera.position = new Vector3(10, 15, 10);
		}

		public void updateManual()
		{
			updateCamera();
			if(Input.GetKey(KeyCode.LeftControl))
				if(Input.GetMouseButtonUp(0))
					_human.buyUnit(Libraries.Utility.Random.Next(_human.faction.units.Count));
			updateSelection();
			updateControls();
		}
		private void updateCamera()
		{
			_position_increment=new Vector3(0, Input.GetAxis("Mouse ScrollWheel")*-speed_zoom*MathF.Log(transform_camera.position.y+1), 0);
			{
				Vector3 _pos_temp = transform_camera.forward;
				_pos_temp.y=0;
				//_pos_temp.Normalize();
				_position_increment+=transform_camera.right*Input.GetAxis("Horizontal")*transform_camera.position.y*speed_move
									+_pos_temp*Input.GetAxis("Vertical")*transform_camera.position.y*speed_move;
			}
			_position_increment*=Time.deltaTime;

			if(Input.GetKey(KeyCode.LeftShift))
			{
				_position_increment.x*=3f;
				_position_increment.z*=3f;
			}
			_position_next=transform_camera.position+_position_increment;
			if(_position_next.x<_limits.x && _position_next.x>0 && _position_next.y<_limits.y && _position_next.z<_limits.z&&_position_next.z>0 && _position_next.y>TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z]*_limits.y+5)
			{
				//if(_position_next.y<=TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z])	//REVIEW:
				//_position_next.y=Math.Min(TerrainGenerator.height_map[(int)_position_next.x, (int)_position_next.z]+5, _limits.y);
				transform_camera.position=_position_next;
			}

			if(Input.GetMouseButtonDown(1))
			{
				_mouse_position_1=Input.mousePosition;
			}
			if(Input.GetMouseButton(1))
			{
				_mouse_position_2=Input.mousePosition;
				_rotation_increment=(_mouse_position_2-_mouse_position_1)*speed_rotate;
				transform_camera.rotation*=Quaternion.Euler(new Vector3(0, _rotation_increment.x, 0));
				transform_camera.GetChild(0).transform.rotation*=Quaternion.Euler(new Vector3(-_rotation_increment.y, 0, 0));
				_mouse_position_1=_mouse_position_2;
			}
		}
		private void updateSelection()
		{
			if(Input.GetMouseButtonDown(0))
			{
				_human._selection.is_select_on_click=true;
				_human._selection._mouse_position_1 = Input.mousePosition;
			}
			if(Input.GetMouseButton(0))
			{
				if((_human._selection._mouse_position_1-Input.mousePosition).magnitude>40)
				{
					_human._selection.is_select_on_click=false;
					_human._selection._mouse_position_2=Input.mousePosition;
				}
			}
			if(Input.GetMouseButtonUp(0))
			{
				if(_human._selection.is_select_on_click)
					_human._selection.selectOnClick(Input.GetKey(KeyCode.LeftControl));
				else
					_human._selection.selectOnDrag(Input.GetKey(KeyCode.LeftControl));
				_human._selection.is_select_on_click=true;
			}
		}
		private void updateControls()
		{
			if(Input.GetMouseButtonDown(1))
			{
				_mouse_behavior.time_rmb_pressed = Time.timeSinceLevelLoad;
				_mouse_behavior.is_rmb_held = false;
			}
			else if(Input.GetMouseButtonUp(1))
			{
				if(!_mouse_behavior.is_rmb_held)
					if(_human.units_selected.Count>0)
						_human.giveOrder(Input.mousePosition);
				_mouse_behavior.is_rmb_held = false;
			}
			if(Input.GetMouseButton(1))
				if(Time.timeSinceLevelLoad-_mouse_behavior.time_rmb_pressed > _mouse_behavior.duration_rmb_held)
					_mouse_behavior.is_rmb_held = true;
		}
	}
}

