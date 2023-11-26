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

	//private Human _human;
	private HumanSelectionDrawer _drawer;
	public Dictionary<int, DynamicObject> selected_objects = new Dictionary<int, DynamicObject>();
	private MeshCollider _selection_collider;
	private RaycastHit _raycast_hit;
	private Ray _ray;
	private Vector2[] _selection_collider_corners;
	public Vector3 _mouse_position_1, _mouse_position_2;
	private Vector3 _selection_collider_corner_left_bottom, _selection_collider_corner_right_top;
	private Vector3[] _vertices = new Vector3[4], _lines = new Vector3[4];
	public bool is_select_on_click;

	void Start()
	{
		_drawer=new HumanSelectionDrawer(Color.green);
		_selection_collider=gameObject.AddComponent<MeshCollider>();
		_selection_collider.convex=true;
		_selection_collider.isTrigger=true;
	}
	/*void Update()
	{
		if(Input.GetMouseButtonDown(0))
			_mouse_position_1 = Input.mousePosition;
		if(Input.GetMouseButton(0))
			if((_mouse_position_1-Input.mousePosition).magnitude>40)
				_select_multiple=true;
		_selection_collider.enabled=true;
		if(Input.GetMouseButtonUp(0))
		{
			if(!_select_multiple)
			{
				_ray = Camera.main.ScreenPointToRay(_mouse_position_1);
				if(Physics.Raycast(_ray, out _raycast_hit, 50000.0f))
				{
					if(Input.GetKey(KeyCode.LeftControl))
						addSelectedObject(_raycast_hit.transform.gameObject);
					else
						deselectAllObjects();
					addSelectedObject(_raycast_hit.transform.gameObject);
				}
				else
					if(!Input.GetKey(KeyCode.LeftControl))
					deselectAllObjects();
			}
			else
			{
				_mouse_position_2 = Input.mousePosition;
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
					if(Physics.Raycast(_ray, out _raycast_hit, 50000.0f, 3))        //TODO: deal with layers
					{
						_vertices[i]=new Vector3(_raycast_hit.point.x, _raycast_hit.point.y, _raycast_hit.point.z);
						_lines[i]=_ray.origin - _raycast_hit.point;
						Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), _raycast_hit.point, Color.red, 10.0f, true);
					}
					i++;
				}
				_selection_collider.sharedMesh = _drawer.generateSelectionMesh(_vertices, _lines);
				_selection_collider.enabled=true;

				if(!Input.GetKey(KeyCode.LeftControl))
					deselectAllObjects();

				_select_multiple = false;
			}
		}
	}*/

	private void OnGUI()	//NOTE no ways to call OnGUI manually
	{
		if(!is_select_on_click)
		{
			_drawer.drawUiRectangle(_mouse_position_1, _mouse_position_2);
		}
	}
	private void OnTriggerEnter(Collider collider)
	{
		addSelectedObject(collider.gameObject.GetComponent<DynamicObject>());
	}

	public void selectOnClick(bool is_adding_to_current_selection)
	{
		if(!is_adding_to_current_selection)
			deselectObjects();
		if(Physics.Raycast(Camera.main.ScreenPointToRay(_mouse_position_1), out _raycast_hit, 50000.0f))
			addSelectedObject(_raycast_hit.transform.gameObject.GetComponent<DynamicObject>());
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
			if(Physics.Raycast(_ray, out _raycast_hit, 50000.0f, 3))        //TODO: deal with layers
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
		yield return new WaitForSeconds(1.0f);
		_selection_collider.enabled = false;
	}
	public void addSelectedObject(DynamicObject obj)
	{
		if(obj!=null)
		{
			int id = obj.GetInstanceID();

			if(!selected_objects.ContainsKey(id))
			{
				selected_objects.Add(id, obj);
				obj.gameObject.AddComponent<SelectionComponent>();
				Debug.Log("Added " + id + " to selected dict");
			}
		}
	}
	public void deselectObjects(int id=-1)	//Remove one or remove all 
	{
		if(id>-1)
		{
			Destroy(selected_objects[id].GetComponent<SelectionComponent>());
			selected_objects.Remove(id);
		}
		else
		{
			foreach(KeyValuePair<int, DynamicObject> obj in selected_objects)
				if(obj.Value != null)
					Destroy(selected_objects[obj.Key].GetComponent<SelectionComponent>());
			selected_objects.Clear();
		}
	}
}
