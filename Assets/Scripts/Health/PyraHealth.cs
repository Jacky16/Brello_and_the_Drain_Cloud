using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public sealed class PyraHealth : Health
{
    Vector3 closestPointToRespawn;
    NavMeshAgent agent;
    bool isProtected;
    // Start is called before the first frame update
    private void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void ResetStats()
    {
        base.ResetStats();

        agent.enabled = false;
        transform.position = closestPointToRespawn;
        agent.enabled = true;
    }

    public void SetClosestPoint(Vector3 pos)
    {
        closestPointToRespawn = pos;
    }
    public void SetIsProtected(bool value)
    {
        isProtected = value;
    }

    public bool GetIsProtected()
    {
        return isProtected;
    }
}
