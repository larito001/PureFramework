using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class EnemyRangeTrigger : MonoBehaviour
{
    public int Id { get;private set; }
    private void Awake()
    {
       var rig= GetComponent<Rigidbody>();
       rig.useGravity = false;

    }

    public void Init(int id)
    {
        this.Id = id;
    }
    
}
