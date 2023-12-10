using System.Collections.Generic;

using UnityEngine;

public sealed partial class Human : Player
{
    private HumanInput _input;
	private HumanSelection _selection;
	private RaycastHit _raycast_hit;
	public Dictionary<int, Unit> units_selected = new Dictionary<int, Unit>();	//
	public DynamicObject object_selected;

	public override void AwakeManual(Team team, string nickname, Faction faction)
	{
		base.AwakeManual(team, nickname, faction);	
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
	public override void giveOrder(Vector3 position)
	{
		if(Physics.Raycast(Camera.main.ScreenPointToRay(position), out _raycast_hit, 5000.0f, 1<<6 | 1<<3 | 1<<7 | 1<<8))
		{
			DynamicObject target=_raycast_hit.transform.gameObject.GetComponent<DynamicObject>();
			foreach(Unit unit in units_selected.Values)
			{
				unit.setOrder(_raycast_hit.point, target);
			}
		}
	}

}
