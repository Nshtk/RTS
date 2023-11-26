using UnityEngine;

namespace Units.Air
{
    public sealed class AirExample : AirUnit
    {
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
