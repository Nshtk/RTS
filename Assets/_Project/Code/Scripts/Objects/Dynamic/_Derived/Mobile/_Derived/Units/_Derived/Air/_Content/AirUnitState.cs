using UnityEngine;

public partial class AirnUnit : Unit
{
	public abstract class AirUnitState : UnitState
	{

		public AirUnitState(Unit unit) : base(unit)
		{
		}

		public override void enter()
		{

		}
		public override void update()
		{

		}
		public override void exit()
		{
		}
	}
	public class AirUnitIdleState : AirUnitState
	{
		public AirUnitIdleState(Unit unit) : base(unit)
		{

		}

		public override void update()
		{
			base.update();
		}
	}
}
