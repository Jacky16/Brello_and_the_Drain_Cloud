using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraPuzzle : MonoBehaviour
{
    [SerializeField] private PyraController pyra;
    [SerializeField] private PyraPuzzleExit exit;
    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject torchParticles;
    private bool isCompleted;

    // Update is called once per frame
    void Update()
    {
        if (isCompleted)
        {
            torchParticles.SetActive(true);
        }
    }

    public PyraController GetAttachedPyra()
    {
        return pyra;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            PyraPuzzleManager.instance.SetCurrentPuzzle(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            PyraPuzzleManager.instance.SetCurrentPuzzle(null);
        }
    }
}
