using System.Collections.Generic;
using System.Linq;
using Units.Ground;
using UnityEngine;

public class Faction     // TODO faction-specific modifiers?
{
	public string name;
    public Texture2D? flag;
	public List<Unit>? units=new List<Unit>();

	public Faction(string name, Texture2D? flag =null, List<Unit>? units=null)
	{
		this.name = name;
		this.flag = flag;
		this.units = units;
		;
	}
}
