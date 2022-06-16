using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPuzzle : MonoBehaviour
{
    [SerializeField] List<Crystal> crystals = new List<Crystal>();

    private int crystalsToComplete = 0;

    private int shinyCrystals = 0;

    [SerializeField] GameObject dropply;

    private void Awake()
    {
        dropply.SetActive(false);
    }

    private void Start()
    {
        foreach(Crystal crystal in crystals)
        {
            if(crystal.isShiny)
            crystalsToComplete++;
        }
    }

    public void ResetCrystals()
    {
        if(crystalsToComplete > 0)
        AkSoundEngine.PostEvent("Deactivate", gameObject);

        foreach (Crystal crystal in crystals)
        {
            StartCoroutine(crystal.ResetCrystal());
            shinyCrystals = 0;
        }
    }

    public void AddGoodOne()
    {
        shinyCrystals++;

        

        if(shinyCrystals == crystalsToComplete)
        {
            CompletePuzzle();
        }
    }


    private void CompletePuzzle()
    {
        dropply.SetActive(true);
    }

}
