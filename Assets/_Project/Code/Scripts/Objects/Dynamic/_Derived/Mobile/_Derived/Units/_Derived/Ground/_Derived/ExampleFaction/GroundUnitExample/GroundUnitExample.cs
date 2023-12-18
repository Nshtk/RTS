using UnityEngine;

namespace Units.Ground
{
	public sealed class GroundUnitExample : GroundUnit
	{
		public override string Name
		{
			get { return "GroundUnitExample"; }
		}

		protected override void Awake()
		{
			base.Awake();
			cost_money=15;
			movement_type=MOBILE_OBJECT_MOVEMENT_TYPE.TRACKED;
			type=UNIT_TYPE.VEHICLE_LIGHT;
		}
		protected override void Start()
		{
			base.Start();
		}
		protected override void Update()
		{
			base.Update();
		}

		public override void setOrder(Vector3 position, DynamicObject target = null)
		{
			if (state_current==state_idle)
			{
				destination=position;
				this.target=target;
				if (target!=null)
				{
					if (target.GetComponent<Unit>() is Unit unit)
					{
						if (unit.owner.team==owner.team)
							changeState(state_follow);
						else
							changeState(state_engage);
					}
				}
				else if (destination!=null)
					changeState(state_evade);
			}
			else
				changeState(state_idle);
		}
	}
}
