using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraProtection : MonoBehaviour
{
    bool isInRainZone;
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask pyraMask;
    PyraHealth pyra;
    BrelloOpenManager openManager;

    private void Start()
    {
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraHealth>();
        openManager = GetComponent<BrelloOpenManager>();
        isInRainZone = false;
    }
    private void FixedUpdate()
    {
        if (openManager.GetIsOpen())
        {
            if (Physics.CheckSphere(transform.position, detectionRadius, pyraMask))
            {
                pyra.SetIsProtected(true);
            }
            else
            {
                pyra.SetIsProtected(false);
            }
        }
        else
        {
            pyra.SetIsProtected(false);
        }
    }

    public bool GetIsInRain()
    {
        return isInRainZone;
    }

    public void SetIsInRain(bool value)
    {
        isInRainZone = value;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
