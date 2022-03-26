using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    string actualTerrainType = "Default";

    public void PlayFootstep()
    {
        //RaycastHit hit;
        //
        //if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)){
        //
        //    if (hit.collider != null) {
        //        if (hit.collider.transform.gameObject.TryGetComponent(out Terrain terrain))
        //        {
        //            actualTerrainType = terrain.GetTerrainType();
        //            Debug.Log(actualTerrainType);
        //        }
        //    }
        //}

        //AkSoundEngine.SetSwitch("FootstepTerrainType", actualTerrainType, WwiseManager.instance.gameObject);
        //AkSoundEngine.PostEvent("Footstep_Brello", WwiseManager.instance.gameObject);
    }
}
