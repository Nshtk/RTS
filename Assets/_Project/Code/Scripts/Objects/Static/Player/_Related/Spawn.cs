using UnityEngine;
using Libraries.Utility;

public class Spawn : MonoBehaviour
{
	[SerializeField] private int length;
	[SerializeField] private int height;
	[SerializeField] private int width;
	private Player _player_owner;
	private BoxCollider _collider;

	public void AwakeManual(Player owner, TerrainGenerator.POSITION_DOCK_SIDE? position_dock_side)
	{
		_player_owner = owner;
		gameObject.transform.localScale=new Vector3(length, width, height);
		_collider=gameObject.GetComponent<BoxCollider>();
		Game.instance.Terrain_Generator.setPositionOnMapRandom(gameObject, transform.localScale, 'y');
		if(position_dock_side!=null)
			Game.instance.Terrain_Generator.setPositionOnMapDocked(gameObject, position_dock_side.Value);
	}
	private void Awake()
	{
		
	}
	private void Start()
	{

	}
	private void Update()
	{
		
	}
	private void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.GetComponent<Unit>() is Unit unit && unit!=null)
		{
			if(unit._player_owner.id==_player_owner.id)
				unit.repair();
		}

	}
	public Unit spawnUnit(Unit unit, Vector3? position=null) //NOTE: cant have one var of type Vector3
	{
		Unit obj_new;
		Vector3 obj_position;

		if(position==null)
		{
			obj_position=Utility.getRandomPointInCollider(_collider);
			obj_position.y = Terrain.activeTerrain.SampleHeight(obj_position) + Terrain.activeTerrain.GetPosition().y;
			obj_position.y += 0.5f;
		}
		else
			obj_position=position.Value;
		obj_new=Instantiate(unit, obj_position, Quaternion.identity); obj_new.initialise(_player_owner);

		return obj_new;
	}
}
