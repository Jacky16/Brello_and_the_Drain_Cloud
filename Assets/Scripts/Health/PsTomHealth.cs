using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsTomHealth : Health
{
    PsTom psTom;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        psTom = GetComponent<PsTom>();
    }
    
    protected override void onDamage()
    {
        base.onDamage();
        Debug.ClearDeveloperConsole();
        print("Vida actual de Pstom:" + currLife);
        psTom.ChangePhase(currLife);
    }
    protected override void onDeath()
    {
        base.onDeath();
        anim.SetBool("IsDeath", true);
    }
}
