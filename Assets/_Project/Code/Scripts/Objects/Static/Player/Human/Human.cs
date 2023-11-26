using UnityEngine;

public sealed partial class Human : Player
{
    private HumanInput _input;
	private HumanSelection _selection;

	public override void AwakeManual(string nickname, Team team, Faction faction)
	{
		base.AwakeManual(nickname, team, faction);	
	}
	protected override void Awake()
	{
		base.Awake();
		_input=new HumanInput(this);
		_selection=gameObject.GetComponent<HumanSelection>();
	}
	protected override void Start()
    {
        base.Start();

    }
	protected override void Update()
    {
        base.Update();
		_input.updateManual();
	}

}
