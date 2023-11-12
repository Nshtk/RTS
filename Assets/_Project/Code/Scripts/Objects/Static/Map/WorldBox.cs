using UnityEngine;

public class WorldBox : MonoBehaviour
{
	private void Awake()
    {
		Collider[] colliders = GetComponents<Collider>();
		Mesh mesh = GetComponent<MeshFilter>().mesh;

		for(int i = 0; i < colliders.Length; i++)        // REVIEW
			DestroyImmediate(colliders[i]);

		int[] triangles = mesh.triangles;
		for(int i = 0, t; i < triangles.Length; i+=3)
		{
			t = triangles[i];
			triangles[i] = triangles[i + 2];
			triangles[i + 2] = t;
		}
		mesh.triangles= triangles;
		gameObject.AddComponent<MeshCollider>();
	}
    private void Start()
    {

	}
    private void Update()
    {
        
    }

	public void initialise(int length, int width, int height, Vector3 terrain_data_size)
	{
		//Transform transform = GetComponent<Transform>();
		transform.position=new Vector3(length/2, -width, height/2);
		transform.localScale=new Vector3(terrain_data_size.x, terrain_data_size.y*4, terrain_data_size.z);
	}

}
