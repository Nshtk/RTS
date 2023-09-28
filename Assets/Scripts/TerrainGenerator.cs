using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	private Terrain _terrain;

	public int length=256;
	public int width=256;
	public int height = 20;
	public float scale = 20f;

	private void Start()
	{
		_terrain = GetComponent<Terrain>();
		_terrain.terrainData.heightmapResolution = width+1;
		_terrain.terrainData.size=new Vector3(length, height, width);
		_terrain.terrainData.SetHeights(0, 0, generateHeightMap());
	}
	private void Update()
	{
		
	}

	private float[,] generateHeightMap() 
	{
		float[,] height_map = new float[length, width];

		for(int i = 0; i < length; i++)
			for(int j = 0; j < width; j++)
				height_map[i, j]=Mathf.PerlinNoise((float)i/length*scale, (float)j/width*scale);
				
		return height_map;
	}
}
