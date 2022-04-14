using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class OffMeshLinkManager : MonoBehaviour
{
    private float jumpDuration;

    [SerializeField]
    [Range(0, 7)]
    float maxHeightJump;

    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;

        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(Jump(agent, maxHeightJump));
            }
            agent.CompleteOffMeshLink();
            yield return null;
        }
    }

    IEnumerator Jump(NavMeshAgent agent, float height)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        jumpDuration = Vector3.Distance(startPos, endPos) / (agent.speed * 1.5f);

        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = height * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / jumpDuration;
            yield return null;
        }
    }
}
