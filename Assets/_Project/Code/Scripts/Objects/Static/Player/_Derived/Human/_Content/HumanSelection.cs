using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanSelection : MonoBehaviour
{
	public class HumanSelectionDrawer
	{
		private Mesh _mesh = new Mesh();
		private Vector3[] _mesh_vertices = new Vector3[8];
		private Texture2D _texture = new Texture2D(3, 3);
		private Color[] _colors;
		private GUIStyle _style = new GUIStyle();
		private Rect _rect;
		private Vector3 _rect_corner_left_top, _rect_corner_right_bottom;

		public HumanSelectionDrawer(Color color_selection, int texture_border_thickness = 2)
		{
			_colors=Enumerable.Repeat(color_selection, _texture.width*_texture.height).ToArray();
			_texture.filterMode = FilterMode.Point;
			_texture.SetPixels(_colors);
			_texture.SetPixel(1, 1, Color.clear);       //Center pixel
			_texture.Apply();
			_style.border = new RectOffset(texture_border_thickness, texture_border_thickness, texture_border_thickness, texture_border_thickness);
			_style.normal.background = _texture;
		}

		public Mesh generateSelectionMesh(Vector3[] corners, Vector3[] lines)
		{
			int i = 0;
			for(; i<4; i++)
				_mesh_vertices[i]=corners[i];
			for(; i<8; i++)
				_mesh_vertices[i]=corners[i-4]+lines[i-4];
			_mesh.vertices=_mesh_vertices;
			_mesh.triangles=new int[] { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };   //NOTE: Set triangles after verts!

			return _mesh;
		}
		public void drawUiRectangle(Vector3 mouse_position_1, Vector3 mouse_position_2)
		{
			mouse_position_1.y=Screen.height-mouse_position_1.y;
			mouse_position_2.y=Screen.height-mouse_position_2.y;
			_rect_corner_left_top = Vector3.Min(mouse_position_1, mouse_position_2);
			_rect_corner_right_bottom = Vector3.Max(mouse_position_1, mouse_position_2);
			_rect = Rect.MinMaxRect(_rect_corner_left_top.x, _rect_corner_left_top.y, _rect_corner_right_bottom.x, _rect_corner_right_bottom.y);
			GUI.Box(_rect, GUIContent.none, _style);
		}
	}
	private LayerMask _layer_mask_click_selection, _layer_mask_drag_selection;
	private Human _human;
	private HumanSelectionDrawer _drawer;
	private MeshCollider _selection_collider;
	private RaycastHit _raycast_hit;
	private Ray _ray;
	private Vector2[] _selection_collider_corners;
	public Vector3 _mouse_position_1, _mouse_position_2;
	private Vector3 _selection_collider_corner_left_bottom, _selection_collider_corner_right_top;
	private Vector3[] _vertices = new Vector3[4], _lines = new Vector3[4];
	public bool is_select_on_click=true;

	void Start()
	{
		_human=gameObject.GetComponent<Human>();
		_drawer=new HumanSelectionDrawer(Color.green);
		_selection_collider=gameObject.AddComponent<MeshCollider>();
		_selection_collider.convex=true;
		_selection_collider.isTrigger=true;
		_layer_mask_click_selection=1<<3 | 1<<7 | 1<<8; _layer_mask_drag_selection=1<<3 | 1<<8;	//PlaneRaycast, Building, Unit
	}

	private void OnGUI()	//NOTE: no ways to call OnGUI manually
	{
		if(!is_select_on_click)
			_drawer.drawUiRectangle(_mouse_position_1, _mouse_position_2);
	}
	private void OnTriggerEnter(Collider collider)
	{
		DynamicObject obj = collider.gameObject.GetComponent<DynamicObject>();
		if(obj!=null)
		{
			if (obj is Unit unit)
			{
				if(unit.owner.id==_human.id)
					selectObject(unit);
			}
			else
				selectObject(obj);
		}
	}

	public void selectOnClick(bool is_adding_to_current_selection)
	{
		if(!is_adding_to_current_selection)
			deselectObjects();
		if (Physics.Raycast(Camera.main.ScreenPointToRay(_mouse_position_1), out _raycast_hit, 50000.0f, _layer_mask_click_selection))
			selectObject(_raycast_hit.transform.gameObject.GetComponent<DynamicObject>(), is_adding_to_current_selection);
		Debug.DrawLine(Camera.main.ScreenToWorldPoint(_mouse_position_1), _raycast_hit.point, Color.red, 10.0f, true);
	}
	public void selectOnDrag(bool is_adding_to_current_selection)
	{
		if(!is_adding_to_current_selection)
			deselectObjects();
		_selection_collider_corner_left_bottom = Vector3.Min(_mouse_position_1, _mouse_position_2);
		_selection_collider_corner_right_top = Vector3.Max(_mouse_position_1, _mouse_position_2);
		_selection_collider_corners=new Vector2[] {
			new Vector2(_selection_collider_corner_left_bottom.x, _selection_collider_corner_right_top.y),		//top left
			new Vector2(_selection_collider_corner_right_top.x, _selection_collider_corner_right_top.y),		//top right
			new Vector2(_selection_collider_corner_left_bottom.x, _selection_collider_corner_left_bottom.y),	//bottom left
			new Vector2(_selection_collider_corner_right_top.x, _selection_collider_corner_left_bottom.y)		//bottom right
		};

		int i = 0;
		foreach(Vector2 corner in _selection_collider_corners)
		{
			_ray=Camera.main.ScreenPointToRay(corner);
			if (Physics.Raycast(_ray, out _raycast_hit, 50000.0f, _layer_mask_drag_selection))        //TODO: deal with layers
			{
				_vertices[i]=new Vector3(_raycast_hit.point.x, _raycast_hit.point.y, _raycast_hit.point.z);
				_lines[i]=_ray.origin - _raycast_hit.point;
				Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), _raycast_hit.point, Color.red, 10.0f, true);
			}
			i++;
		}
		_selection_collider.sharedMesh = _drawer.generateSelectionMesh(_vertices, _lines);
		_selection_collider.enabled=true;
		StartCoroutine(disableSelectionCollider());
	}

	private IEnumerator disableSelectionCollider()
	{
		yield return new WaitForSeconds(1f);
		_selection_collider.enabled = false;
		is_select_on_click=true; //REVIEW
	}
	public void selectObject(DynamicObject obj, bool is_adding_to_current_selection=false)
	{
		if(obj!=null)
		{
			if (obj is Unit unit)
			{
				int id = unit.GetInstanceID();
				if (!_human.units_selected.ContainsKey(id))
					_human.units_selected.Add(id, unit);
				else
					if(is_adding_to_current_selection)
						deselectObjects(id);
			}
			else
				_human.object_selected=obj;
			obj.gameObject.AddComponent<SelectionComponent>();
		}
	}
	public void deselectObjects(int id=-1)	//Remove one or remove all 
	{
		if(id>-1)
		{
			Destroy(_human.units_selected[id].GetComponent<SelectionComponent>());
			_human.units_selected.Remove(id);
		}
		else
		{
			foreach(KeyValuePair<int, Unit> obj in _human.units_selected)
				if(obj.Value != null)
					Destroy(_human.units_selected[obj.Key].GetComponent<SelectionComponent>());
			_human.units_selected.Clear();
		}
	}
}
