using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour
{
    [SerializeField] public Texture2D flag;
    // TODO faction-specific modifiers?
    public List<Unit> units;
    private void Awake()
    {
        units= new List<Unit>() {
            new Units.Ground.ExampleGround(),
			new Units.Air.ExampleAir()
		};
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
}
