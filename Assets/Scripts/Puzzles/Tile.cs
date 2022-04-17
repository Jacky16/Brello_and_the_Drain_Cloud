using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // El orden en el que deben ser pisados
    [SerializeField] int order;

    // Tiempo que duran en rojo al fallar
    [SerializeField] float errorTime;

    // Manager del puzzle
    private TilesPuzzle puzzleManager;

    // Booleano para ignorar colisiones y que se pueda resetear más facilmente
    // y además que el jugador pueda volver por casillas ya verdes
    [HideInInspector]
    public bool isActive = true;

    private void Start()
    {
        puzzleManager = GetComponentInParent<TilesPuzzle>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isActive)
        {

            // CompareOrder es una función del manager y devuelve true si se sigue el orden correcto
            // Analize result es una función para ordenar el codigo
            AnalizeResult(puzzleManager.CompareOrder(order));
        }
    }


    private void AnalizeResult(bool result)
    {
        if (result)
        {
            // Se pone la casilla verde y ya no se puede volver a pisar para facilitar las cosas
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            isActive = false;
        }
    }

    public void Reset()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.gray;
        isActive = true;
    }

    private IEnumerator Error()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        isActive = false;

        yield return new WaitForSeconds(errorTime);

        Reset();
    }

    public void DoError()
    {
        StartCoroutine(Error());
    }
}
