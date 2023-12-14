using UnityEngine;
using Libraries.Terrain;
using Libraries;
using Unity.AI.Navigation;
using UnityStandardAssets.Water;
using Libraries.Utility;

public sealed class TerrainGenerator : MonoBehaviour
{
	[SerializeField] private WaterBasic prefab_water;
	public enum POSITION_DOCK_SIDE	//REVIEW:
	{
		NORTH,
		SOUTH,
		WEST,
		EAST
	}
	[SerializeField] private WorldBox _worldbox;
	[SerializeField] private PlaneRaycast _plane_raycast;
	private Terrain _terrain;
	public Map map;
	private NavMeshSurface[] _navmesh_surfaces;

	[Header("Generation parametrs")]
	public int length = 256;
	public int height = 256;
	public int width = 20;
	public float scale = 20f;
	public float[,] height_map;
	public float[,,] alpha_map;

	private void Awake()
	{
		_navmesh_surfaces=GetComponents<NavMeshSurface>();
	}
	public void AwakeManual()
	{
		height_map = new float[length, height];
		alpha_map = new float[257, 257, 9];
		_terrain = Terrain.activeTerrain;
		map = new Map(length, height);
		map.generateRandom();

		_terrain.terrainData.alphamapResolution=length+1;
		_terrain.terrainData.heightmapResolution=length+1;
		_terrain.terrainData.size=new Vector3(length, width, height);
		_terrain.terrainData.SetHeights(0, 0, height_map);
		_terrain.terrainData.SetAlphamaps(0, 0, alpha_map);

		Instantiate(_worldbox).initialise(length, width, height, _terrain.terrainData.size);
		Instantiate(_plane_raycast).initialise(length, width, height, _terrain.terrainData.size);
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
	public void setPositionOnMapRandom(GameObject obj, Vector3? bounds=null, char ignore_axis='0')
	{
		float position_previous_x=0, position_previous_y=0, position_previous_z=0;
		switch (ignore_axis)
		{
			case 'x':
				position_previous_x=obj.transform.position.x;
				break;
			case 'y':
				position_previous_y=obj.transform.position.y;
				break;
			case 'z':
				position_previous_z=obj.transform.position.z;
				break;
			default:
				break;
		}
		if (bounds!=null)
			obj.transform.position=Utility.getRangedVector3(bounds.Value.x, length-bounds.Value.x, bounds.Value.y, width-bounds.Value.y, bounds.Value.z, height-bounds.Value.z);
		else
			obj.transform.position=Utility.Random.NextVector3(length, width, height);
		switch (ignore_axis)
		{
			case 'x':
				obj.transform.position=new Vector3(position_previous_x, obj.transform.position.y, obj.transform.position.z);
				break;
			case 'y':
				obj.transform.position=new Vector3(obj.transform.position.x, position_previous_y, obj.transform.position.z);
				break;
			case 'z':
				obj.transform.position=new Vector3(obj.transform.position.x, obj.transform.position.y, position_previous_z);
				break;
			default:
				break;
		}
	}
	public void setPositionOnMapDocked(GameObject obj, POSITION_DOCK_SIDE side)
	{
		switch(side)	//FIXME
		{
			case POSITION_DOCK_SIDE.NORTH:
				obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, length-obj.transform.localScale.z/2);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, -180, 0));
				break;
			case POSITION_DOCK_SIDE.SOUTH:
				obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.localScale.z/2);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case POSITION_DOCK_SIDE.WEST:
				obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, transform.localScale.z/2);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case POSITION_DOCK_SIDE.EAST:
				obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, height-transform.localScale.z/2);
				obj.transform.rotation=Quaternion.Euler(new Vector3(0, -90, 0));
				break;
			default:
				break;
		}
	}
	public void createWater(Vector3 position, float water_level)
	{

	}
}
/*public class AssignSplatMap : MonoBehaviour	//TODO grab useful stuff
{
	void Start()
	{
		// Get the attached terrain component
		Terrain terrain = GetComponent<Terrain>();

		// Get a reference to the terrain data
		TerrainData terrainData = terrain.terrainData;

		// Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
		float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

		for (int y = 0; y < terrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < terrainData.alphamapWidth; x++)
			{
				// Normalise x/y coordinates to range 0-1 
				float y_01 = (float)y/(float)terrainData.alphamapHeight;
				float x_01 = (float)x/(float)terrainData.alphamapWidth;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));

				// Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
				Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

				// Calculate the steepness of the terrain
				float steepness = terrainData.GetSteepness(y_01, x_01);

				// Setup an array to record the mix of texture weights at this point
				float[] splatWeights = new float[terrainData.alphamapLayers];

				// CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

				// Texture[0] has constant influence
				splatWeights[0] = 0.5f;

				// Texture[1] is stronger at lower altitudes
				splatWeights[1] = Mathf.Clamp01((terrainData.heightmapResolution - height));

				// Texture[2] stronger on flatter terrain
				// "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
				// Subtract result from 1.0 to give greater weighting to flat surfaces
				splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapResolution/5.0f));

				// Texture[3] increases with height but only on surfaces facing positive Z axis 
				splatWeights[3] = height * Mathf.Clamp01(normal.z);

				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
				float z = splatWeights.Sum();

				// Loop through each terrain texture
				for (int i = 0; i<terrainData.alphamapLayers; i++)
				{

					// Normalize so that sum of all texture weights = 1
					splatWeights[i] /= z;

					// Assign this point to the splatmap array
					splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}

		// Finally assign the new splatmap to the terrainData:
		terrainData.SetAlphamaps(0, 0, splatmapData);
	}
}*/
