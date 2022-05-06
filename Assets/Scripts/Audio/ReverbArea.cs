using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.SetRTPCValue("IsReverbOn", 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.SetRTPCValue("IsReverbOn", 0f);
        }
    }
}
