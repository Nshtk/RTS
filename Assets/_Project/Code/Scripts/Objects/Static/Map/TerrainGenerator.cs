using UnityEngine;
using Libraries.Terrain;
using Libraries;
using Unity.AI.Navigation;

public class TerrainGenerator : MonoBehaviour
{
	public enum POSITION_DOCK_SIDE	//REVIEW:
	{
		NORTH,
		SOUTH,
		WEST,
		EAST
	}
	[SerializeField] private WorldBox _worldbox;
	public static TerrainGenerator instance;
	private Terrain _terrain;
	private Map _map;
	private NavMeshSurface[] _navmesh_surfaces;

	[Header("Generation parametrs")]
	public int length = 256;
	public int height = 256;
	public int width = 20;
	public float scale = 20f;
	public static float[,] height_map;
	public static float[,,] alpha_map;

	private void Awake()
	{
		_navmesh_surfaces=GetComponents<NavMeshSurface>();
	}
	public void AwakeManual()
	{
		instance=this;
		height_map = new float[length, height];
		alpha_map = new float[257, 257, 9];
		_terrain = Terrain.activeTerrain; // _terrain = GetComponent<Terrain>();
		_map = new Map(length, height);
		//_terrain.terrainData.GetAlphamaps(0, 0, _terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight);
		_map.generateRandom();

		_terrain.terrainData.alphamapResolution=length+1;
		_terrain.terrainData.heightmapResolution=length+1;
		_terrain.terrainData.size=new Vector3(length, width, height);
		_terrain.terrainData.SetHeights(0, 0, height_map);
		_terrain.terrainData.SetAlphamaps(0, 0, alpha_map);

		Instantiate(_worldbox).initialise(length, width, height, _terrain.terrainData.size);
	}
	private void Start()
	{
		for(int i=0; i<_navmesh_surfaces.Length; i++)
			_navmesh_surfaces[i].BuildNavMesh();
	}
	public void StartManual()
	{

	}
	private void Update()
	{

	}
	public void UpdateManual()
	{
		
	}
	public void setRandomPositionOnMap(GameObject obj, Vector3? bounds=null)
	{
		if(bounds!=null)
			obj.transform.position=Utility.getRangedVector3(bounds.Value.x, length-bounds.Value.x, bounds.Value.y, width-bounds.Value.y, bounds.Value.z, height-bounds.Value.z);
		else
			obj.transform.position=Utility.Random.NextVector3(length, width, height);
	}
	public void setDockedPositionOnMap(GameObject obj, POSITION_DOCK_SIDE side)
	{
		switch(side)
		{
			case POSITION_DOCK_SIDE.NORTH:
				obj.transform.position = new Vector3(length-obj.transform.localScale.x/2, obj.transform.position.y, obj.transform.position.z);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, -180, 0));
				break;
			case POSITION_DOCK_SIDE.SOUTH:
				obj.transform.position = new Vector3(length-obj.transform.localScale.x/2, obj.transform.position.y, obj.transform.position.z);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case POSITION_DOCK_SIDE.WEST:
				obj.transform.position = new Vector3(length-obj.transform.localScale.x/2, obj.transform.position.y, obj.transform.position.z);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case POSITION_DOCK_SIDE.EAST:
				obj.transform.position = new Vector3(length-obj.transform.localScale.x/2, obj.transform.position.y, obj.transform.position.z);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, -90, 0));
				break;
			default:
				break;
		}
	}
}
