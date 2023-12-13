using UnityEngine;

namespace Units.Ground
{
	public sealed class GroundUnitExample : GroundUnit
	{
		public override string Name
		{
			get { return "VehicleGround"; }
		}

		protected override void Awake()
		{
			base.Awake();
			cost=15;
			movement_type=MOBILE_OBJECT_MOVEMENT_TYPE.TRACKED;
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
}


