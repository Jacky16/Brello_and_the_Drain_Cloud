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

    //Variable para hacer los cálculos de si se sigue el orden correcto
    int lastTile = 0;

    // Función que es llamada por un tile del puzzle al colisionar con el player
    // la función envía como parámetro su posición en el orden del puzzle 
    // para así poder compararla con la última posicion (var lastTile)
    // y saber si se sigue el orden correcto
    public bool CompareOrder(int actualTile)
    {
        if (lastTile == actualTile - 1)
        {
            lastTile++;

             //Si es el último tile se completa el puzzle
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

    // Función que hace que todas los tiles se vuelvan rojos
    private void WrongCombination()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].DoError(); ;
        }
    }

    // Función que es llamada por los tiles al dar error y así se resetean todos
    public void ResetPuzzle()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Reset();
        }

        lastTile = 0;
    }

    // Funcion para hacer la conclusión y resultado del puzzle
    private void Completed()
    {
        pillar.DOMove(moveTo.position, timeToArrive).SetEase(Ease.InOutSine);
        //Sonido de puzzle completado
        AkSoundEngine.PostEvent("PuzzlePillar", WwiseManager.instance.gameObject);
    }
}
