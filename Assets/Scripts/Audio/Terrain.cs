using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] string TerrainType = "Default";
    
    public string GetTerrainType()
    {
        return TerrainType;
    }

}
