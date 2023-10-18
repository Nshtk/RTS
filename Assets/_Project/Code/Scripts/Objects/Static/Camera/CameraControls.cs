using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
	private Transform _transform;
	public float speed = 3f;

	void Start()
	{
		_transform = GetComponent<Transform>();
	}
	void Update()
	{
		_transform.position+=new Vector3(Input.GetAxis("Horizontal"), -Mouse.current.scroll.y.ReadValue(), Input.GetAxis("Vertical")).normalized * speed * Time.deltaTime;
	}
}
