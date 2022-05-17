using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarHealth : Health
{
    protected override void onDamage()
    {
        animator.SetTrigger("Fall");
    }
}
