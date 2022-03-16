using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraProtection : MonoBehaviour
{
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask pyraMask;
    Collider[] pyra;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        //if (Physics.CheckSphere(transform.position, detectionRadius, pyraMask))
        //{
        //    pyra = Physics.OverlapSphere(transform.position, detectionRadius, pyraMask);
        //    pyra[0].GetComponent<PyraHealth>().SetIsProtected(true);
        //}
        //else
        //{
        //    if (pyra.Length > 0)
        //    {
        //        pyra[0].GetComponent<PyraHealth>().SetIsProtected(false);
        //    }
        //}
    }
}
