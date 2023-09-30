using UnityEngine;
using Libraries.Map;

public class TerrainGenerator : MonoBehaviour
{
	private Terrain _terrain;
	private Map _map;

	public int length = 256;
	public int height = 256;
	public int width = 20;
	public float scale = 20f;

	private void Start()
	{
		_terrain = GetComponent<Terrain>();
		_map = new Map(length, height);
		_terrain.terrainData.heightmapResolution=length+1;
		_terrain.terrainData.size=new Vector3(length, width, height);
		_terrain.terrainData.SetHeights(0, 0, generateHeightMap());
	}
	private void Update()
	{

	}

	private float[,] generateHeightMap()
	{
		_map.generateRandom();
		float[,] height_map = new float[length, height];

		for (int i = 0; i<length; i++)
			for (int j = 0; j<height; j++)
				height_map[i, j]=_map.tiles[i, j].height;
		//height_map[i, j]=Mathf.PerlinNoise((float)i/length*scale, (float)j/height*scale);

		return height_map;
	}
}
