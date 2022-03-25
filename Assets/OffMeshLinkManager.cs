using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class OffMeshLinkManager : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    float jumpDuration;

    [SerializeField]
    [Range(0, 5)]
    float maxHeightJump;

    private Animator animator;


    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;

        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(Jump(agent, maxHeightJump, jumpDuration));
            }
            agent.CompleteOffMeshLink();
            yield return null;
        }
    }

    IEnumerator Jump(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = height * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
