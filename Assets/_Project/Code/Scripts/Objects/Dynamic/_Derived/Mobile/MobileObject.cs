using UnityEngine;

public partial class MobileObject : DynamicObject
{
	public enum MOBILE_OBJECT_MOVEMENT_TYPE
	{
		FREE,
		TRACKED,
		FOOTED,		//TODO add terrain deformation on heavy unit step
		WHEELED,
	}

	protected DynamicObject target = null;
	public MOBILE_OBJECT_MOVEMENT_TYPE movement_type;

	public float maneuverability;
	public Vector3? destination = null;

	protected override void Awake()
	{
		base.Awake();
	}
	protected override void Start()
	{
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}
}
