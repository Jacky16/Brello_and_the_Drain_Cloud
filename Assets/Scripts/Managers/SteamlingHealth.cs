using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SteamlingHealth : Health
{
    private GameObject cloudMask;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //cloudMask = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    protected override void onDamage()
    {    
        if(currLife == 1)
        {
            //Si tiene animacion de daño
            //animator.SetTrigger("Damage");
            //Poner particulas si se quiere o sonido
            cloudMask.SetActive(false);
        }
    }
}
