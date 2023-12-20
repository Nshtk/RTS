using System;

using UnityEngine;

namespace Units.Air
{
	public sealed class AirUnitExample : AirUnit
	{
		AirUnitExampleDiedEventArgs _event_args_air_unit_example_died;

		public override string Name
		{
			get { return "AirUnitExample"; }
		}

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

		protected override void OnDestroy()
		{
			base.OnDestroy();
			airUnitExampleDied?.Invoke(this, (AirUnitExampleDiedEventArgs)_event_args_unit_died);
		}

		public delegate void AirUnitExampleDiedEventHandler(AirUnitExample sender, AirUnitExampleDiedEventArgs e);
		public event AirUnitExampleDiedEventHandler airUnitExampleDied;
		public class AirUnitExampleDiedEventArgs : UnitDiedEventArgs
		{
			public AirUnitExampleDiedEventArgs(Unit unit, MobileObject killer = null, string message_force_override = null) : base(unit, killer, message_force_override)
			{}
		}
	}
}
