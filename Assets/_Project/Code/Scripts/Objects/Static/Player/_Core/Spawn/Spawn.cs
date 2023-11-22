using UnityEngine;
using Libraries;

public class Spawn : MonoBehaviour
{
    [SerializeField] private int length;
	[SerializeField] private int height;
	private Player _player_owner;
	private BoxCollider _collider;

    private void Start()
    {
		_collider=gameObject.GetComponent<BoxCollider>();
    }
    private void Update()
    {
        
    }
	private void OnTriggerEnter(Collider collider_other)
	{
		if(collider_other.gameObject.GetComponent<Unit>() is Unit unit && unit!=null)
        {
			//if(unit.player_id==_player_id)		// TODO: repair units
		}

	}
	public void initialise(Player owner, Vector3 position)
	{
		_player_owner = owner;
		gameObject.transform.position = position;
	}
	public Unit spawnUnit(Unit unit) 
    {
        Unit obj_new=unit;
        Vector3 obj_position=Utility.getRandomPointInCollider(GetComponent<BoxCollider>());
        float terain_height=TerrainGenerator.height_map[(int)obj_position.x, (int)obj_position.z]*TerrainGenerator.instance.width;

        if(obj_position.y<terain_height)
			obj_position.y=terain_height+3;
        obj_new=Instantiate(obj_new); obj_new.initialise(_player_owner, obj_position);

        return obj_new;
	}
}
