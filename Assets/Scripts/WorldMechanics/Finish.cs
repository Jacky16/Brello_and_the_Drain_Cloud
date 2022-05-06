using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] string nameScene;

    [SerializeField] BackgroundMusic bm;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController pc))
        {
            AkSoundEngine.PostEvent("StopBackgroundMusic_Level1", WwiseManager.instance.gameObject);
            SceneManager.LoadScene(nameScene);
        }
    }
}
