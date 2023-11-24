using UnityEngine;

public sealed partial class Human : Player
{
    HumanInput human_input;
	protected override void Awake()
	{
		base.Awake();
		human_input=new HumanInput(this);
	}
	protected override void Start()
    {
        base.Start();

    }
	protected override void Update()
    {
        base.Update();
		human_input.updateManual();
	}

	public override void StartManual(Faction faction)
	{
		base.StartManual( faction);
	}
}
