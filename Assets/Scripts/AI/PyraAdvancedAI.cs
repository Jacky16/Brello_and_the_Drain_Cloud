using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraAdvancedAI : MonoBehaviour
{
    PyraAI pyraOutOfCombat;
    PyraCombatAI pyraInCombat;
    CombatManager combatManager;
    private void Start()
    {
        pyraOutOfCombat = GetComponent<PyraAI>();
        pyraInCombat = GetComponent<PyraCombatAI>();
        combatManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatManager>();
    }

    private void Update()
    {
        if (combatManager.GetIsInCombat())
        {
            pyraInCombat.enabled = true;
            pyraOutOfCombat.enabled = false;
        }
        else
        {
            pyraInCombat.enabled = false;
            pyraOutOfCombat.enabled = true;
        }
    }
}
