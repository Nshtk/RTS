using UnityEngine;

public class PlaneRaycast : MonoBehaviour
{
	private void Awake()
	{
		
	}
	private void Start()
	{

	}

	public void initialise(int length, int width, int height, Vector3 terrain_data_size)
	{
		transform.position=new Vector3(length/2, -width/1.5f, height/2);
		transform.localScale=new Vector3(terrain_data_size.x*0.15f, 1, terrain_data_size.z*0.15f);
		var c=gameObject.AddComponent<BoxCollider>();
		c.transform.localScale=new Vector3(terrain_data_size.x*2, 1, terrain_data_size.x*2);
	}
}
