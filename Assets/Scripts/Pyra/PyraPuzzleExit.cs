using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraPuzzleExit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PyraController pyra))
        {
            pyra.SetIsInExit(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PyraController pyra))
        {
            pyra.SetIsInExit(false);
        }
    }
}
