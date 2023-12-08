using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class animationScriptController : MonoBehaviour
{
    Object obj;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Solder");
        animator = obj.GetComponent<Animator>();
        Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
        // Согласовать различные кейсы, после синхронизации с физикой и механикой юнитов

        // If you run out of ammo

    }
}
