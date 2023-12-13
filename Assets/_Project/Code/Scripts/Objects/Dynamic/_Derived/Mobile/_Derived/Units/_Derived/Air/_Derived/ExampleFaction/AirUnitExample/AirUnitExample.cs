using UnityEngine;

namespace Units.Air
{
    public sealed class AirUnitExample : AirUnit
    {
		public override string Name
		{
			get { return "VehicleAir"; }
		}

		protected override void Awake()
		{
			base.Awake();
			cost=15;
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
