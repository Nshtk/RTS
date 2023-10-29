using UnityEngine;

public class SelectionComponent : MonoBehaviour
{
	private Material _default;

	public SelectionComponent()
	{
		_default=GetComponent<Renderer>().material;
	}

	private void Start()
    {
		GetComponent<Renderer>().material.color=Color.red;
	}
    private void Update()
    {
		GetComponent<Renderer>().material.color=_default.color;
	}
}
