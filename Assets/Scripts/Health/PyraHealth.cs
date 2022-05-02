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

    protected void ResetStats()
    {
        currLife = maxLife;
        isInmune = false;

        //AkSoundEngine.PostEvent("Death_Pyra", WwiseManager.instance.gameObject);

        agent.enabled = false;
        transform.position = closestPointToRespawn;
        agent.enabled = true;

    }
    private IEnumerator Reappear()
    {
        yield return new WaitForSeconds(timeToReappear);
        ResetStats();
    }

    protected override void onDeath()
    {
        animator.SetTrigger("Die");
        StartCoroutine(Reappear());
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
