using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    public void PSTomAngry()
    {
        AkSoundEngine.PostEvent("Angry_PSTom", WwiseManager.instance.gameObject);
    }

    public void PSTomCrash()
    {
        AkSoundEngine.PostEvent("Crash_PSTom", WwiseManager.instance.gameObject);
    }
}
