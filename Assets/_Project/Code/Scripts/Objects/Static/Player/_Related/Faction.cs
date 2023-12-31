using System.Collections.Generic;
using UnityEngine;

public class Faction     // TODO faction-specific modifiers?
{
	public string name;
	public Texture2D? flag;
	public List<Unit>? units=new List<Unit>();
	public HashSet<Unit.UNIT_TYPE> unit_types=new HashSet<Unit.UNIT_TYPE>();
	string path = "Objects\\Dynamic\\Derived\\Mobile\\Derived\\Units\\Derived\\";	//REVIEW pathing

	public Faction(string name, Texture2D? flag=null)
	{
		this.name = name;
		if(flag!=null)		//REVIEW:
			this.flag=flag;
		units.AddRange(new List<GroundUnit>(Resources.LoadAll<GroundUnit>(path+"Ground\\Derived\\"+name)));
		units.AddRange(new List<AirUnit>(Resources.LoadAll<AirUnit>(path+"Air\\Derived\\"+name)));

		foreach(Unit unit in units)
		{
			if(!unit_types.Contains(unit.type))
				unit_types.Add(unit.type);
		}
	}
}
