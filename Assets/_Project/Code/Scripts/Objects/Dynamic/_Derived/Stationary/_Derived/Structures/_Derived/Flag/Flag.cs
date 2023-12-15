using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flag : Structure
{
    public enum FLAG_sTATUS
    {
        CLEAR,
        DEFENDED,
        DEFENDED_STRONG,
        ATTACKED,
        ATTACKED_STRONG
    }

	public int status;
	public Player player_owner;
    public HashSet<Unit> flag_units;
    public float progress;

    public void initialise(Vector3 size)
    {
        transform.localScale = size;
    }
    protected override void Awake()
    {
		flag_units=new HashSet<Unit>();
        progress=0;
	}
	protected override void Start()
    {
        base.Start();
    }
	protected override void Update()
    {
        base.Update();
    }
    public override void UpdateManual()
    {
		flagCaptured?.Invoke(this, new FlagCapturedEventArgs($"{Id} has been captured."));
	}
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null) 
        {
			flag_units.Add(unit);
		}
	}
	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null)
		{
            if(flag_units.Contains(unit))
				flag_units.Remove(unit);
		}
	}
    public void updateStatus()
    {
        foreach (Unit unit in flag_units) 
        {
            //if(unit.)
        }
    }
    public void updateFlagUnits()
    {
		Collider[] colliders = Physics.OverlapBox(GetComponent<Renderer>().bounds.center, transform.localScale);
	}
	public List<Unit> getFlagUnits()
	{
        return flag_units.ToList();
	}

	public delegate void FlagCapturedEventHandler(Flag sender, FlagCapturedEventArgs e);
	public static event FlagCapturedEventHandler flagCaptured;
	public class FlagCapturedEventArgs : EventArgs
	{
		public string Message
		{
			get;
			set;
		}
		public FlagCapturedEventArgs(string message)
		{
			Message = message;
		}
	}
}
