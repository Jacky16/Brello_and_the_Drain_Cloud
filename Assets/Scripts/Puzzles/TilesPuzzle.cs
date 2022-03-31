using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TilesPuzzle : MonoBehaviour
{
    // Los 9 tiles del puzzle
    [SerializeField] Tile[] tiles = new Tile[9];

    //Variables para el resultado del puzzle
    [SerializeField] Transform pillar;
    [SerializeField] Transform moveTo;
    [SerializeField] float timeToArrive;

    //Variable para hacer los c�lculos de si se sigue el orden correcto
    int lastTile = 0;

    // Funci�n que es llamada por un tile del puzzle al colisionar con el player
    // la funci�n env�a como par�metro su posici�n en el orden del puzzle 
    // para as� poder compararla con la �ltima posicion (var lastTile)
    // y saber si se sigue el orden correcto
    public bool CompareOrder(int actualTile)
    {
        if (lastTile == actualTile - 1)
        {
            lastTile++;

             //Si es el �ltimo tile se completa el puzzle
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

    // Funci�n que hace que todas los tiles se vuelvan rojos
    private void WrongCombination()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].DoError(); ;
        }
    }

    // Funci�n que es llamada por los tiles al dar error y as� se resetean todos
    public void ResetPuzzle()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Reset();
        }

        lastTile = 0;
    }

    // Funcion para hacer la conclusi�n y resultado del puzzle
    private void Completed()
    {
        pillar.DOMove(moveTo.position, timeToArrive).SetEase(Ease.InOutSine);
        //Sonido de puzzle completado
        AkSoundEngine.PostEvent("PuzzlePillar", WwiseManager.instance.gameObject);
    }
}
