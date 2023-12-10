using UnityEngine;

public sealed class Bot : Player
{
	public string strategy;
	public string doctrine;

	public override void AwakeManual(Team team, string nickname, Faction faction)
	{
		base.AwakeManual(team, nickname, faction);
		if (nickname==null)
			this.nickname=GetInstanceID().ToString();
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
	public override void giveOrder(Vector3 position)
	{

	}
}
