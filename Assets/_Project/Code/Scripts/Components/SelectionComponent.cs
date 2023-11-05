using cakeslice;
using UnityEngine;

public class SelectionComponent : MonoBehaviour
{
	//private Color _color_default;

	private void Awake()
	{
		//_color_default = GetComponent<Renderer>().material.color;	// NOTE: Not allowed in constructor
	}
	private void Start()
    {
		//GetComponent<Renderer>().material.color=Color.red;
		gameObject.AddComponent<Outline>();
	}
    private void OnDestroy()
    {
		Destroy(GetComponent<Outline>());
		//GetComponent<Renderer>().material.color=_color_default;
	}
}
