using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{


    public void PlayPSTomAngry()
    {
        AkSoundEngine.PostEvent("Angry_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomCrash()
    {
        AkSoundEngine.PostEvent("Crash_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomPunch()
    {
        AkSoundEngine.PostEvent("Punch_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomHit()
    {
        AkSoundEngine.PostEvent("Hit_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomLanding()
    {
        AkSoundEngine.PostEvent("Landing_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomCharging()
    {
        AkSoundEngine.PostEvent("Charging_PSTom", WwiseManager.instance.gameObject);
    }

    public void PlayPSTomCharge()
    {
        AkSoundEngine.PostEvent("Charge_PSTom", WwiseManager.instance.gameObject);
    }
}
