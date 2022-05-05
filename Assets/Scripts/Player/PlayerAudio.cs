using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    string actualTerrainType = "Default";

    [System.NonSerialized]
    public int isReverbOn = 0;

    private void Start()
    {
        //AkSoundEngine.PostEvent("BackgroundMusic_Level1", WwiseManager.instance.gameObject);
    }

    public void PlayFootstep()
    {
        actualTerrainType = "Default";

        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)){
        
            if (hit.collider != null) {
                if (hit.collider.transform.gameObject.TryGetComponent(out Terrain terrain))
                {
                    actualTerrainType = terrain.GetTerrainType();
                    //Debug.Log(actualTerrainType);
                }
            }
        }
    
        //AkSoundEngine.SetSwitch("FootstepSurfaceType", actualTerrainType, WwiseManager.instance.gameObject);
        //AkSoundEngine.PostEvent("Footstep_Brello", WwiseManager.instance.gameObject);
    }

    public void PlayAttack()
    {
        //AkSoundEngine.PostEvent("Attack_Combo_Brello", WwiseManager.instance.gameObject);
    }

    public void PlayStartGlide()
    {
        //AkSoundEngine.PostEvent("Start_Glide_Brello", WwiseManager.instance.gameObject);
    }

    public void StopGlide()
    {
        //AkSoundEngine.PostEvent("Stop_Glide_Brello", WwiseManager.instance.gameObject);
    }

    public void PlayOpen()
    {
        //AkSoundEngine.PostEvent("Open_Brello", WwiseManager.instance.gameObject);
    }

    public void PlayClose()
    {
        //AkSoundEngine.PostEvent("Close_Brello", WwiseManager.instance.gameObject);
    }
}
