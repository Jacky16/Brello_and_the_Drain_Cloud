using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesPuzzle : MonoBehaviour
{
    [SerializeField] Tile[] tiles = new Tile[9];

    [SerializeField] Transform pillar;
    [SerializeField] Transform moveTo;

    int lastTile = 0;

    public bool CompareOrder(int actualTile)
    {
        if (lastTile == actualTile - 1)
        {
            lastTile++;

            if (lastTile == 9)
            {
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].isActive = false;
                }

                Completed();
            }

            return true;
        }
        else
        {
            lastTile = 0;
            WrongCombination();
            return false;
        }
    }

    private void WrongCombination()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].DoError(); ;
        }
    }

    public void ResetPuzzle()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Reset();
        }

        lastTile = 0;
    }

    private void Completed()
    {

    }
}
