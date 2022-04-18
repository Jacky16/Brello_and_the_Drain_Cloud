using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsTomHealth : Health
{
    PsTom psTom;

    protected override void Start()
    {
        base.Start();
        psTom = GetComponent<PsTom>();

    }
    protected override void onDamage()
    {
        base.onDamage();
        Debug.ClearDeveloperConsole();
        print("Vida actual de Pstom:" + currLife);
        psTom.ChangePhase(currLife);
    }
}
