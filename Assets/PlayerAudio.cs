using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField]
    LayerMask groundLayerMask;

    [SerializeField]
    private AK.Wwise.Switch footstepSurfaceType;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void PlayAttack()
    {
        footstepSurfaceType.SetValue(WwiseManager.instance.gameObject); 
        AkSoundEngine.PostEvent("Attack_Combo_Brello", WwiseManager.instance.gameObject);
    }

    private void PlayFootstep()
    {
        AkSoundEngine.PostEvent("Footstep_Brello", WwiseManager.instance.gameObject);
    }

    public void ChangeSurfaceType()
    {
        ReturnSurfaceType();
    }

    public void Open()
    {

    }

    public void Glide()
    {

    }

    public void ReturnSurfaceType()
    {
        AkSoundEngine.SetSwitch("FootstepSurfaceType", "Default", WwiseManager.instance.gameObject);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    footstepSurfaceType.SetValue(gameObject);
                    AkSoundEngine.SetSwitch("FootstepSurfaceType", "Grass", WwiseManager.instance.gameObject);
                }
            }
        }
    }

}
