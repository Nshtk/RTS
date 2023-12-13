using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour
{
	protected string[] actions = { };
	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	protected GUIStyle healthStyle = new GUIStyle();
	protected Rigidbody _rigid_body;

	public Texture2D icon;
	public AudioClip select_sound;
	private List<Material> oldMaterials = new List<Material>();

	public ulong id;
	public int hit_points = 100, hit_points_max = 100;
	protected float healthPercentage = 1.0f;
	public int repair_rate;
	public float hardness;
	public float resistance;
	public float mass;
	public float temperature;
	public float select_volume = 1f;

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
	protected virtual void FixedUpdate()
	{

	}
	protected virtual void OnGUI()
	{
	
	}
	protected virtual void OnDestroy()
	{
	}
	public virtual void initialise(Player owner)
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
}
