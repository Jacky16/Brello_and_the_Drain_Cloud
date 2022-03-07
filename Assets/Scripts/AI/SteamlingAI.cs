using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamlingAI : EnemyAI
{
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    protected override void Update()
    {
        base.Update();
    }
    protected override void AttackAction()
    {
        if (agent.hasPath)
        {
            Debug.Log("Entro");
            agent.acceleration = (agent.remainingDistance < attackRadius) ? deceleration : acceleration;
        }
    }
}
