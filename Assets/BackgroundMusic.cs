using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public void StopMusic()
    {
        AkSoundEngine.PostEvent("StopAll", gameObject);
    }
}
