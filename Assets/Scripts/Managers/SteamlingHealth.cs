using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SteamlingHealth : Health
{
    private GameObject cloudMask;
    private MeshRenderer rendererSteamling;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //cloudMask = transform.GetChild(0).gameObject;
        rendererSteamling = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    protected override void onDamage()
    {   
        if(currLife == 1)
        {
            //Si tiene animacion de da�o
            //animator.SetTrigger("Damage");
            //Poner particulas si se quiere o sonido
            //cloudMask.SetActive(false);
            
        }
        rendererSteamling.material.SetColor("_MainColor", Color.red);
        StartCoroutine(ResetColor());

        AkSoundEngine.PostEvent("Hurt_Steamling", WwiseManager.instance.gameObject);
    }

    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.4f);
        rendererSteamling.material.SetColor("_MainColor", Color.white);
    }
    protected override void onDeath()
    {
        Destroy(gameObject);
    }
}
