using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirImpulser : MonoBehaviour
{
    [SerializeField] private float impulseForce;



    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().Move(transform.up * impulseForce * Time.deltaTime);
        }
    }
}
