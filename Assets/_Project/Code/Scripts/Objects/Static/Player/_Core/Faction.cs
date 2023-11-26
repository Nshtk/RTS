using System.Collections.Generic;
using UnityEngine;

public class Faction     // TODO faction-specific modifiers?
{
	public string name;
    public Texture2D? flag;
	public List<Unit>? units=new List<Unit>();

	public Faction(string name, Texture2D? flag=null)
	{
		this.name = name;
		if(flag!=null)		//REVIEW:
			this.flag=flag;
		units.AddRange(new List<GroundUnit>(Resources.LoadAll<GroundUnit>(name)));
		units.AddRange(new List<AirUnit>(Resources.LoadAll<AirUnit>(name)));
	}
}
