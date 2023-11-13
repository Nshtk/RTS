using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Player Body Parts")]
	public int id;
	public string nickname;
	public Team team;
	public Faction faction;
	private Spawn spawn;

	public int money, money_income, money_pool;
	public int difficulty_multiplier;

	public List<Unit> units=new List<Unit>();

	protected virtual void Start()
    {
        
    }
	public virtual void StartManual()
	{
		
	}
	protected virtual void Update()
    {
		money+=money_income;
		money_pool-=money_income;
    }
	public virtual void UpdateManual()
	{

	}

	/*public void SaveDetails(JsonWriter writer)
	{
		SaveManager.WriteString(writer, "Username", username);
		SaveManager.WriteBoolean(writer, "Human", human);
		SaveManager.WriteColor(writer, "TeamColor", teamColor);
		SaveManager.SavePlayerResources(writer, resources, resourceLimits);
		SaveManager.SavePlayerBuildings(writer, GetComponentsInChildren<Building>());
		SaveManager.SavePlayerUnits(writer, GetComponentsInChildren<Unit>());
	}*/

	/*public void LoadDetails(JsonTextReader reader)
	{
		if(reader == null)
			return;
		string currValue = "";
		while(reader.Read())
		{
			if(reader.Value!=null)
			{
				if(reader.TokenType == JsonToken.PropertyName)
				{
					currValue = (string)reader.Value;
				}
				else
				{
					switch(currValue)
					{
						case "Username":
							username = (string)reader.Value;
							break;
						case "Human":
							human = (bool)reader.Value;
							break;
						default:
							break;
					}
				}
			}
			else if(reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray)
			{
				switch(currValue)
				{
					case "TeamColor":
						teamColor = LoadManager.LoadColor(reader);
						break;
					case "Resources":
						LoadResources(reader);
						break;
					case "Buildings":
						LoadBuildings(reader);
						break;
					case "Units":
						LoadUnits(reader);
						break;
					default:
						break;
				}
			}
			else if(reader.TokenType==JsonToken.EndObject)
				return;
		}
	}*/
}
