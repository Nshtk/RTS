using UnityEngine;

public class Team : MonoBehaviour
{
	public int id;
	public Texture2D? flag;
	public Color color;
	//public Goal goal; TODO 

	public Team(int id, Color color, Texture2D? flag=null)
	{
		this.id=id;
		this.color=color;
		if(flag!=null)
			this.flag=flag;
	}

	public void getUnitInfo()
	{
	
	}
	public void haveUnit() // args find units by such properties as name, cost etc
	{

	}
	public void getFlagUnits()
	{

	}
}
