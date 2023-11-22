using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DynamicObject : MonoBehaviour
{
	public string object_name = "WorldObject";
	public Texture2D icon;
	public int hit_points = 100, hit_points_max = 100;
	public float detectionRange = 20.0f;
	public AudioClip select_sound;
	public float select_volume = 1f;

	protected Player player;
	protected string[] actions = { };
	protected bool currentlySelected = false;
	protected Bounds selectionBounds;
	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	protected GUIStyle healthStyle = new GUIStyle();
	protected float healthPercentage = 1.0f;
	protected DynamicObject target = null;
	protected bool attacking = false, movingIntoPosition = false, aiming = false;
	protected bool loadedSavedValues = false;
	protected List<DynamicObject> nearbyObjects;

	private List<Material> oldMaterials = new List<Material>();
	//private int loadedTargetId = -1;

	public int ObjectId
	{
		get;
		set;
	}
	public Texture2D Icon
	{
		get;
		set;
	}

	protected virtual void Awake()
	{

	}
	protected virtual void Start()
	{

	}
	public virtual void StartManual()
	{

	}
	protected virtual void Update()
	{

	}
	public virtual void UpdateManual()
	{

	}
	protected virtual void OnGUI()
	{
	
	}
	protected virtual void OnDestroy()
	{
	}
	public virtual void initialise(Player owner, Vector3 obj_position)
	{
	}
	protected virtual void InitialiseAudio()
	{
		List<AudioClip> sounds = new List<AudioClip>();
		List<float> volumes = new List<float>();
		/*if(attackVolume < 0.0f)
			attackVolume = 0.0f;
		if(attackVolume > 1.0f)
			attackVolume = 1.0f;
		sounds.Add(attackSound);
		volumes.Add(attackVolume);
		if(selectVolume < 0.0f)
			selectVolume = 0.0f;
		if(selectVolume > 1.0f)
			selectVolume = 1.0f;
		sounds.Add(selectSound);
		volumes.Add(selectVolume);
		if(useWeaponVolume < 0.0f)
			useWeaponVolume = 0.0f;
		if(useWeaponVolume > 1.0f)
			useWeaponVolume = 1.0f;
		sounds.Add(useWeaponSound);
		volumes.Add(useWeaponVolume);
		audioElement = new AudioElement(sounds, volumes, objectName + ObjectId, this.transform);*/
	}
	public void setSelected(bool is_selected)
	{
		
	}
	public void CalculateBounds()
	{
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
		{
			selectionBounds.Encapsulate(r.bounds);
		}
	}
	public void SetTransparentMaterial(Material material, bool storeExistingMaterial)
	{
		if(storeExistingMaterial)
			oldMaterials.Clear();
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers)
		{
			if(storeExistingMaterial)
				oldMaterials.Add(renderer.material);
			renderer.material = material;
		}
	}

	public void RestoreMaterials()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		if(oldMaterials.Count == renderers.Length)
		{
			for(int i = 0; i<renderers.Length; i++)
			{
				renderers[i].material = oldMaterials[i];
			}
		}
	}

	/*public virtual void SaveDetails(JsonWriter writer)
	{
		SaveManager.WriteString(writer, "Type", name);
		SaveManager.WriteString(writer, "Name", objectName);
		SaveManager.WriteInt(writer, "Id", ObjectId);
		SaveManager.WriteVector(writer, "Position", transform.position);
		SaveManager.WriteQuaternion(writer, "Rotation", transform.rotation);
		SaveManager.WriteVector(writer, "Scale", transform.localScale);
		SaveManager.WriteInt(writer, "HitPoints", hitPoints);
		SaveManager.WriteBoolean(writer, "Attacking", attacking);
		SaveManager.WriteBoolean(writer, "MovingIntoPosition", movingIntoPosition);
		SaveManager.WriteBoolean(writer, "Aiming", aiming);
		if(attacking)
		{
			//only save if attacking so that we do not end up storing massive numbers for no reason
			SaveManager.WriteFloat(writer, "CurrentWeaponChargeTime", currentWeaponChargeTime);
		}
		if(target != null)
			SaveManager.WriteInt(writer, "TargetId", target.ObjectId);
	}

	public void LoadDetails(JsonTextReader reader)
	{
		while(reader.Read())
		{
			if(reader.Value != null)
			{
				if(reader.TokenType == JsonToken.PropertyName)
				{
					string propertyName = (string)reader.Value;
					reader.Read();
					HandleLoadedProperty(reader, propertyName, reader.Value);
				}
			}
			else if(reader.TokenType == JsonToken.EndObject)
			{
				//loaded position invalidates the selection bounds so they must be recalculated
				selectionBounds = ResourceManager.InvalidBounds;
				CalculateBounds();
				loadedSavedValues = true;
				return;
			}
		}
		//loaded position invalidates the selection bounds so they must be recalculated
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
		loadedSavedValues = true;
	}

	protected virtual void HandleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		switch(propertyName)
		{
			case "Name":
				objectName = (string)readValue;
				break;
			case "Id":
				ObjectId = (int)(System.Int64)readValue;
				break;
			case "Position":
				transform.localPosition = LoadManager.LoadVector(reader);
				break;
			case "Rotation":
				transform.localRotation = LoadManager.LoadQuaternion(reader);
				break;
			case "Scale":
				transform.localScale = LoadManager.LoadVector(reader);
				break;
			case "HitPoints":
				hitPoints = (int)(System.Int64)readValue;
				break;
			case "Attacking":
				attacking = (bool)readValue;
				break;
			case "MovingIntoPosition":
				movingIntoPosition = (bool)readValue;
				break;
			case "Aiming":
				aiming = (bool)readValue;
				break;
			case "CurrentWeaponChargeTime":
				currentWeaponChargeTime = (float)(double)readValue;
				break;
			case "TargetId":
				loadedTargetId = (int)(System.Int64)readValue;
				break;
			default:
				break;
		}
	}*/
}
