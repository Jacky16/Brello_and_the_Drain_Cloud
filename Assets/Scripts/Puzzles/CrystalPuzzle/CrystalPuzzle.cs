using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPuzzle : MonoBehaviour
{
    [SerializeField] Crystal[] crystals = new Crystal[6];


    public void Evaluate()
    {

    }

    private void ResetCrystals()
    {
        foreach(Crystal crystal in crystals)
        {
            crystal.Reset();
        }
    }

}
