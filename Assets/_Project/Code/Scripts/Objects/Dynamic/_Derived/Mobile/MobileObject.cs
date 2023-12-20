using UnityEngine;

public partial class MobileObject : DynamicObject
{
	public enum MOBILE_OBJECT_MOVEMENT_TYPE
	{
		FREE,
		TRACKED,
		FOOTED,     //TODO add terrain deformation on heavy unit step
		WHEELED,
	}

	public MOBILE_OBJECT_MOVEMENT_TYPE movement_type;
	protected DynamicObject target = null;

	public AudioClip sound_move;
	public AudioClip sound_impact;

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

	protected virtual void OnCollisionEnter(Collision collision)
	{
		_audio_source.PlayOneShot(sound_impact);
	}
}
