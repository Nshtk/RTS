using UnityEngine;
using System;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

public class MainLoop : MonoBehaviour
{

	[SerializeField] private Human _human_prefab;
	//private Bot _bot_prefab;
	private List<Player> players=new List<Player>(2);	//NOTE: initial capacity reduces memory reallocations

	private void Awake()
	{
		for(int i=0; i<1; i++)
		{
			Human human = Instantiate(_human_prefab); human.initialise();
			players.Add(human);
		}
			
	}
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
	private void LateUpdate()
	{

	}
	private void FixedUpdate()
	{

	}

	private void OnGUI()
	{

	}
}
