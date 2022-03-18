using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseManager : MonoBehaviour
{
    public static WwiseManager instance;

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else if(instance != null && instance != this)
        {
            Destroy(this);
        }

    }
}
