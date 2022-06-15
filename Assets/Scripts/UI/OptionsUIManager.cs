using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUIManager : MonoBehaviour
{
    OptionsManager optionsManager;
    void OnEnable()
    {
        optionsManager = transform.GetComponentInParent<OptionsManager>();
        optionsManager.LoadUISettings();
    }    
}
