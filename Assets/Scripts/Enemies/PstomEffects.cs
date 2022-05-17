using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsTomEffects : MonoBehaviour
{
    [SerializeField] GameObject vfxXocas;
    [SerializeField] Transform vfxXocasTransform;

    [SerializeField] GameObject vfxJump;
    [SerializeField] Transform vfxJumpTransform;

    [SerializeField] GameObject vfxGroundJump;
    [SerializeField] Transform vfxGroundJumpTransform;
    
    //No esta todo,pueden faltar más efectos

    public void PlayXocas()
    {
        if (vfxXocas != null)
        {
            GameObject vfx = Instantiate(vfxXocas, vfxXocasTransform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }
    public void PlayJump()
    {
        if (vfxJump != null)
        {
            GameObject vfx = Instantiate(vfxJump, vfxJumpTransform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }
    public void PlayGroundJump()
    {
        if (vfxGroundJump != null)
        {
            GameObject vfx = Instantiate(vfxGroundJump, vfxGroundJumpTransform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }

}
