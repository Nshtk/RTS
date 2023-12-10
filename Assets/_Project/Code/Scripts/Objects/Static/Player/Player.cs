using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Player Body Parts")]
	public int id;
	public string nickname;
	public Team team;
	public Faction faction;     //TODO: dont store it use gamedata
	public Spawn spawn;

	public int money, money_income, money_pool = 1000;
	public int difficulty_multiplier;

	public List<Unit> units = new List<Unit>();


	protected virtual void Awake()
	{

	}
	protected virtual void Start()
	{

	}
	public virtual void AwakeManual(Team team, string? nickname, Faction faction)
	{
		spawn=Instantiate(Game.instance.Prefab_Spawn); spawn.AwakeManual(this, team.position_dock_side);
		this.team = team;
		if (nickname==null)
			this.nickname ="Unknown";
		else
			this.nickname=nickname;
		this.nickname = nickname;
		this.faction=faction;
	}
	protected virtual void Update()
	{

	}
	public virtual void UpdateManual()
	{
		if (money_pool>=money_income)
		{
			money+=money_income;
			money_pool-=money_income;
		}
	}
	public virtual void buyUnit(int id)
	{
		if (money>=faction.units[id].cost)
		{
			money-=faction.units[id].cost;
			spawn.spawnUnit(faction.units[id]);
		}

	}
	public virtual void giveOrder(Vector3 position)
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
