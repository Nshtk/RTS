using Libraries;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public partial class Unit : MobileObject
{
	public enum UNIT_STATE
	{
		IDLE,
		FOLLOWING,
		EVADING,
		PATROLING,
		ENGAGING
	}
	public enum UNIT_TYPE	//Unit classes
	{
		INFANTRY_COMMON,
		INFANTRY_TACTICAL,		//Primary anti-infantry
		INFANTRY_VANGUARD,		//Primarily anti-all
		INFANTRY_SABOTEUR,
		INFANTRY_SUPPORT,		//Medics, engineers, scouts, etc.

		VEHICLE_COMMON,			//MG Jeeps, etc.
		VEHICLE_LIGHT,			//APVs, light tanks etc.
		VEHICLE_HEAVY,			//Medium and heavy tanks, etc.
		VEHICLE_ARTILLERY,
		VEHICLE_SUPPORT,		//Transport, repair

		HELICOPTER_SCOUT,		//Light helicopters
		HELICOPTER_ATTACK,

		AIRPLANE_FIGHTER,
		AIRPLANE_BOMBER,

		ULTIMATE
	}

	public Player _player_owner;
	public Inventory inventory;
	protected List<DynamicObject> objects_nearby=new List<DynamicObject>();
	protected UnitController _unit_controller;  //TODO to private
	public AudioClip sound_voiceover, sound_idle, sound_move;
	protected NavMeshPath navmesh_path;
	protected NavMeshQueryFilter navmesh_query_filter;
	public UNIT_STATE state;
	public UNIT_TYPE type;
	public string group;    //For group timers

	public int id_in_faction;
	public int limit=-1;
	public int cost=5;
	public int charge_time=0, recharge_time=0;
	public float experience=0f;
	public float stealth=0f;
	public float detection_range=10f, detection_chance=0.5f;

	protected UnitState state_current;
	protected UnitIdleState state_idle;
	protected UnitFollowState state_follow;
	protected UnitEvadeState state_evade;

	protected bool _is_grounded=false;
	protected bool Is_Grounded
	{
		get 
		{
			return _is_grounded;
		}
		set { _is_grounded = value; }
	}

	public virtual string Name
	{
		get { return "Unit"; }
	}
	
	public override void initialise(Player owner)
	{
		_player_owner=owner;
	}
	protected override void Awake()
	{
		base.Awake();
		_rigidbody = GetComponent<Rigidbody>();
		navmesh_query_filter=new NavMeshQueryFilter() { agentTypeID=GetNavMeshAgentID(), areaMask= 1<<0 | 1<<3};
		navmesh_path =new NavMeshPath();
		_unit_controller=new UnitController(this);
	}
	protected override void Start()
    {
        base.Start();
		unitDied+=Game.GameData.instance.handleUnitDied;
		setStates();
	}
	public override void StartManual()
	{

	}
	protected override void Update()
    {
        base.Update();
		if(Game.instance.game_data.ticks%100==0)
			detectNearbyObjects();
	}
	public override void UpdateManual()
	{
		state_current?.update();
	}
	protected override void FixedUpdate()
	{}
	protected void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<MobileObject>() is MobileObject mobile_object)
		{
			hurt((int)(collision.relativeVelocity.magnitude*(mobile_object.Rigidbody.getKineticEnergy()+_rigidbody.getKineticEnergy())));
		}
		else if (collision.gameObject.GetComponent<Projectile>() is Projectile projectile)
		{
			hurt(projectile.damage);
		}
	}
	protected void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.layer==LayerMask.NameToLayer("Terrain"))	//TODO store in GameData
			Is_Grounded=true;
	}
	protected void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer==LayerMask.NameToLayer("Terrain"))   //TODO store in GameData
			Is_Grounded=false;
	}
	protected void OnTriggerEnter(Collider collider)
	{
		Spawn spawn = collider.gameObject.GetComponent<Spawn>();
		if (spawn!=null)
			repair();
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
	protected int GetNavMeshAgentID()
	{
		for(int i=0; i<NavMesh.GetSettingsCount(); i++)
		{
			NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
			if (Name == NavMesh.GetSettingsNameFromID(settings.agentTypeID))
				return settings.agentTypeID;
		}
		return 1;
	}
	public void setChargeTimer()
	{

	}
	public virtual void setStates()
	{
		state_idle=     new UnitIdleState(this);
		state_follow=   new UnitFollowState(this);
		state_evade=    new UnitEvadeState(this);
		changeState(state_idle);
	}
	public virtual void detectNearbyObjects()
	{
		Collider[] colliders = Physics.OverlapSphere(GetComponent<Renderer>().bounds.center, detection_range);
		objects_nearby.Clear();
		foreach (var collider in colliders)
		{
			DynamicObject dynamic_object=collider.gameObject.GetComponent<DynamicObject>();
			if(dynamic_object!=null)
				objects_nearby.Add(dynamic_object);
		}
	}
	public virtual void setOrder(Vector3 position, DynamicObject target=null)
	{
		if(state_current==state_idle)
		{
			destination=position;
			this.target=target;
			if (target!=null)
			{
				if(target.GetComponent<Unit>() is Unit unit)
				{
					if(unit._player_owner.team==_player_owner.team)
						changeState(state_follow);
					/*else				//FIXME add all possible states in unit for derived to override?
						changeState(state_engage);*/
				}
			}
			else if(destination!=null)
				changeState(state_evade);
		}
		else
			changeState(state_idle);

	}
	public void changeState(UnitState state_next)
	{
		state_current?.exit();
		state_current = state_next;
		state_current.enter();
	}
	public void repair()
	{
		if(hit_points!=hit_points_max)
			hit_points+=repair_rate;
	}
	public void hurt(int damage)
	{
		hit_points-=damage;
		if (hit_points<=0)
		{
			Destroy(gameObject);
			unitDied?.Invoke(this, new UnitDiedEventArgs($"{Name} has died."));
		}

	}

	public delegate void unitDiedEventHandler(Unit sender, UnitDiedEventArgs e);
	public static event unitDiedEventHandler unitDied;
	public class UnitDiedEventArgs : EventArgs
	{
		public string Message
		{
			get;
			set;
		}
		public UnitDiedEventArgs(string message)
		{
			Message = message;
		}
	}
}
