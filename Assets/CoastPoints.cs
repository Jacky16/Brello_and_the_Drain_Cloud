using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastPoints : MonoBehaviour
{
    public List<Transform> coastPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaterPlatformManager.singletone.SetCurrentCoast(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaterPlatformManager.singletone.SetCurrentCoast(null);
        }
    }
}