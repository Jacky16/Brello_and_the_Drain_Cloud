using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPuzzle : MonoBehaviour
{
    [SerializeField] List<Crystal> crystals = new List<Crystal>();

    private int crystalsToComplete = 0;

    private int shinyCrystals = 0;

    private void Awake()
    {
        
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
        foreach (Crystal crystal in crystals)
        {
            StartCoroutine(crystal.ResetCrystal());
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

    }

}
