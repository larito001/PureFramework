using System;
using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation.Hybrid;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = System.Numerics.Vector3;

public class ZombieNavCtrl : MonoBehaviour
{
    private ZombieEntity zombieEntity;
    private Transform Target;
    private AgentAuthoring agent;
    private bool canMove = false;

    private void Awake()
    {
        agent = transform.GetComponent<AgentAuthoring>();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        UnityEngine.Vector3 currentPos = transform.position;
        float distance = math.distance(currentPos, agent.EntityBody.Destination);
        if (agent.EntityBody.IsStopped && distance <= (agent.GetStopDistance() + 0.1f))
        {
            zombieEntity.ZombieMoveStopTrigger();
        }
        else if (!agent.EntityBody.IsStopped && distance > (agent.GetStopDistance() + 0.1f))
        {
            zombieEntity.ZombieMovingTrigger();
        }
        else
        {
            zombieEntity.ZombieMoveStopTrigger();
        }
    }

    public void Stop()
    {
        agent.Stop();
        canMove = false;
    }

    public void StartMove()
    {
        if (Target == null) return;
        canMove = true;
        var body = agent.EntityBody;
        body.Destination = Target.position;
        body.IsStopped = false;
        agent.EntityBody = body;
    }

    public void SetTarget(Transform Target)
    {
        if (canMove && agent)
        {
            this.Target = Target;
            StartMove();
        }

        if (Target == null)
        {
            agent.Stop();
        }
    }

    public void Init(ZombieEntity zombieEntity)
    {
        this.zombieEntity = zombieEntity;
        canMove = true;
    }
}