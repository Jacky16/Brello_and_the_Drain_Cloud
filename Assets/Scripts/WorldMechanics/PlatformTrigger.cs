using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    TutorialManager tutorialManager;
    private void Awake()
    {
        tutorialManager = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.SetLastPlatform(transform.parent.GetComponent<SpacePlatformMovement>());
        }
    }
}
