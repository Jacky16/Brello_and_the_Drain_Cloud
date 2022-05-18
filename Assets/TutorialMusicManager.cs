using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMusicManager : MonoBehaviour
{
    public void StopMusic()
    {
        AkSoundEngine.PostEvent("StopBackgroundMusic_Tutorial", gameObject);
    }
}
