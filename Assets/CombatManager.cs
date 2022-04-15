using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    bool isInCombat;
    public List<GameObject> enemyList;
    // Start is called before the first frame update
    void Start()
    {
        isInCombat = false;
        enemyList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyList.Count > 0)
        {
            isInCombat = true;
        }
        else
        {
            isInCombat = false;
        }
    }

    public bool GetIsInCombat()
    {
        return isInCombat;
    }
}
