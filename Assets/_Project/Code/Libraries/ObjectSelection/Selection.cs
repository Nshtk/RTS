using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
	public class SelectionDrawer
	{
		public Vector2[] getBoundingBox(Vector2 mouse_position_1, Vector2 mouse_position_2)
		{
			Vector3 left_bottom = Vector3.Min(mouse_position_1, mouse_position_2);
			Vector3 right_top = Vector3.Max(mouse_position_1, mouse_position_2);
			Vector2[] corners =
			{
				new Vector2(left_bottom.x, right_top.y),	// top left
				new Vector2(right_top.x, right_top.y),		// top right
				new Vector2(left_bottom.x, left_bottom.y),	// bottom left
				new Vector2(right_top.x, left_bottom.y)		// bottom right
			};
			return corners;
		}
		public Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
		{
			Mesh selectionMesh = new Mesh();
			Vector3[] verts = new Vector3[8];

			selectionMesh.triangles=new int[] {0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7};
			for(int i = 0; i < 4; i++)
				verts[i] = corners[i];
			for(int j = 4; j < 8; j++)
				verts[j] = corners[j - 4] + vecs[j - 4];
			selectionMesh.vertices=verts;

			return selectionMesh;
		}
		public void drawOnScreenRectangle(Rect rectangle, Color color)
		{
			GUI.color = color;
			GUI.DrawTexture(rectangle, WhiteTexture);
			GUI.color = Color.white;
		}
		public void drawOnScreenRectangleBorder(Rect rectangle, float thickness, Color color)
		{
			drawOnScreenRectangle(new Rect(rectangle.xMin, rectangle.yMin, rectangle.width, thickness), color);				// Top
			drawOnScreenRectangle(new Rect(rectangle.xMin, rectangle.yMin, thickness, rectangle.height), color);				// Left
			drawOnScreenRectangle(new Rect(rectangle.xMax - thickness, rectangle.yMin, thickness, rectangle.height), color);	// Right
			drawOnScreenRectangle(new Rect(rectangle.xMin, rectangle.yMax - thickness, rectangle.width, thickness), color);	// Bottom
		}
	}
	public Dictionary<int, GameObject> selected_objects = new Dictionary<int, GameObject>();
	private MeshCollider _selection_box;
	private Mesh _selection_mesh;
	private RaycastHit _raycast_hit;
	private Vector2[] _selection_box_corners;
	private Vector3 _mouse_position_1, _mouse_position_2;
	private Vector3[] _vertices, _lines;
	private static Texture2D _whiteTexture;
	private bool _select_multiple;
	private SelectionDrawer _drawer = new SelectionDrawer();

	public static Texture2D WhiteTexture
	{
		get
		{
			if(_whiteTexture == null)
			{
				_whiteTexture = new Texture2D(1, 1);
				_whiteTexture.SetPixel(0, 0, Color.white);
				_whiteTexture.Apply();
			}
			return _whiteTexture;
		}
	}
	void Start()
	{
		_select_multiple = false;
		_selection_box.convex=true;
		_selection_box.isTrigger=true;
		_selection_box=gameObject.AddComponent<MeshCollider>();
		_whiteTexture = new Texture2D(1, 1);
		_whiteTexture.SetPixel(0, 0, Color.white);
		_whiteTexture.Apply();
	}
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
			_mouse_position_1 = Input.mousePosition;
		if(Input.GetMouseButton(0))
			if((_mouse_position_1-Input.mousePosition).magnitude>40)
				_select_multiple=true;

		if(Input.GetMouseButtonUp(0))
		{
			if(!_select_multiple)
			{
				Ray ray = Camera.main.ScreenPointToRay(_mouse_position_1);
				if(Physics.Raycast(ray, out _raycast_hit, 50000.0f))
				{
					if(Input.GetKey(KeyCode.LeftControl))
						addSelected(_raycast_hit.transform.gameObject);
					else
						deselectAll();
						addSelected(_raycast_hit.transform.gameObject);
				}
				else
					if(!Input.GetKey(KeyCode.LeftControl))
						deselectAll();
			}
			else
			{
				_mouse_position_2 = Input.mousePosition;
				Vector3 left_bottom = Vector3.Min(_mouse_position_1, _mouse_position_2);
				Vector3 right_top = Vector3.Max(_mouse_position_1, _mouse_position_2);
				_selection_box_corners=new Vector2[] {
					new Vector2(left_bottom.x, right_top.y),	// top left
					new Vector2(right_top.x, right_top.y),		// top right
					new Vector2(left_bottom.x, left_bottom.y),	// bottom left
					new Vector2(right_top.x, left_bottom.y)    // bottom right
				};
				_vertices = new Vector3[4];
				_lines = new Vector3[4];

				int i=0;
				foreach(Vector2 corner in _selection_box_corners)
				{
					Ray ray=Camera.main.ScreenPointToRay(corner);
					if(Physics.Raycast(ray, out _raycast_hit, 50000.0f, (1 << 8)))
					{
						_vertices[i]=new Vector3(_raycast_hit.point.x, _raycast_hit.point.y, _raycast_hit.point.z);
						_lines[i]=ray.origin - _raycast_hit.point;
						Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), _raycast_hit.point, Color.red, 1.0f);
					}
					i++;
				}
				_selection_box.gameObject.SetActive(true);
				_selection_mesh=_drawer.generateSelectionMesh(_vertices, _lines);
				_selection_box.sharedMesh=_selection_mesh;
				if(!Input.GetKey(KeyCode.LeftControl))
					deselectAll();
				_selection_box.gameObject.SetActive(false);
				//Destroy(selectionBox, 0.02f);
			}
			_select_multiple = false;
		}
	}
	private void OnGUI()
	{
		if(_select_multiple)
		{
			_mouse_position_2=Input.mousePosition;
			_mouse_position_1.y=Screen.height-_mouse_position_1.y;
			_mouse_position_2.y=Screen.height-_mouse_position_2.y;
			Vector3 corner_left_top=Vector3.Min(_mouse_position_1, _mouse_position_2);
			Vector3 corner_right_bottom=Vector3.Max(_mouse_position_1, _mouse_position_2);
			var rect = Rect.MinMaxRect(corner_left_top.x, corner_left_top.y, corner_right_bottom.x, corner_right_bottom.y);
			_drawer.drawOnScreenRectangle(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
			_drawer.drawOnScreenRectangleBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
		}
	}
	private void OnTriggerEnter(Collider collider)
	{
		addSelected(collider.gameObject);
	}
	public void addSelected(GameObject game_object)
	{
		int id = game_object.GetInstanceID();

		if(!(selected_objects.ContainsKey(id)))
		{
			selected_objects.Add(id, game_object);
			game_object.AddComponent<SelectionComponent>();
			Debug.Log("Added " + id + " to selected dict");
		}
	}
	public void deselect(int id)
	{
		Destroy(selected_objects[id].GetComponent<SelectionComponent>());
		selected_objects.Remove(id);
	}

	public void deselectAll()
	{
		foreach(KeyValuePair<int, GameObject> pair in selected_objects)
			if(pair.Value != null)
				Destroy(selected_objects[pair.Key].GetComponent<SelectionComponent>());
		selected_objects.Clear();
	}
}
