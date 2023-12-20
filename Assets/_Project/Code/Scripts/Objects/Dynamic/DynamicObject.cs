using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour
{
	protected Rigidbody _rigidbody;
	protected AudioSource _audio_source;
	protected GUIStyle style_health = new GUIStyle();
	protected string[] actions = { };

	public Texture2D icon;
	public AudioClip sound_select;
	private List<Material> materials_old = new List<Material>();

	public int hit_points = 100, hit_points_max = 100;
	protected float _hit_points_percentage = 1.0f;
	public int repair_rate;
	public float hardness;
	public float resistance;
	public float mass;
	public float temperature;
	public float select_volume = 1f;

	public int Id
	{
		get;
		set;
	}
	public virtual string Name
	{
		get { return "Dynamic object"; }
	}
	public Texture2D Icon
	{
		get;
		set;
	}
	public Rigidbody Rigidbody
	{
		get { return _rigidbody; }
	}
	public AudioSource Audio_Source
	{
		get { return _audio_source; }
	}
	public int Hit_Points
	{
		get { return hit_points; }
		private set 
		{
			hit_points=value;
			_hit_points_percentage=hit_points/hit_points_max*100;
		}
	}
		
	protected virtual void Awake()
	{
		_rigidbody=GetComponent<Rigidbody>();
		_audio_source=gameObject.AddComponent<AudioSource>();
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
		List<float> volumes = new List<float>();	//TODO dynamicaly change volumes if they exceed 1f or 0f
	}
	public void setSelected(bool is_selected)
	{
		
	}
	public void SetTransparentMaterial(Material material, bool storeExistingMaterial)
	{
		if(storeExistingMaterial)
			materials_old.Clear();
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers)
		{
			if(storeExistingMaterial)
				materials_old.Add(renderer.material);
			renderer.material = material;
		}
	}

	public void RestoreMaterials()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		if(materials_old.Count == renderers.Length)
		{
			for(int i = 0; i<renderers.Length; i++)
			{
				renderers[i].material = materials_old[i];
			}
		}
	}
}
