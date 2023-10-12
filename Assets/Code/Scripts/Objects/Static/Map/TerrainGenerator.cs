using UnityEngine;
using Libraries.Terrain;
using System;

public class TerrainGenerator : MonoBehaviour
{
	private Terrain _terrain;
	public Transform worldbox_transform;
	private Map _map;

	public int length = 256;
	public int height = 256;
	public int width = 20;
	public float scale = 20f;
	public static float[,] height_map;
	public static float[,,] alpha_map;

	private void Start()
	{
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

		worldbox_transform.position=new Vector3(length/2, width/2, height/2);
		worldbox_transform.localScale=_terrain.terrainData.size;
	}
	private void Update()
	{

	}
}
