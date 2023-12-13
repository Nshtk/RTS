using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

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
    public HashSet<Unit> units_flag;
    public float progress;

    private void Awake()
    {
		units_flag=new HashSet<Unit>();
        progress=0;
	}
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null) 
        {
			units_flag.Add(unit);
		}
	}
	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null)
		{
            if(units_flag.Contains(unit))
				units_flag.Remove(unit);
		}
	}

	public List<Unit> getFlagUnits()
	{
        return units_flag.ToList();
	}
}
