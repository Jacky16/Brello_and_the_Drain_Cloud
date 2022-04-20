using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraAudio : MonoBehaviour
{

    public void PlayDeath()
    {
        AkSoundEngine.PostEvent("Death_Pyra", WwiseManager.instance.gameObject);
    }



}
