using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] int order;

    [SerializeField] float errorTime;

    private TilesPuzzle puzzleManager;

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
            AnalizeResult(puzzleManager.CompareOrder(order));
        }
    }


    private void AnalizeResult(bool result)
    {
        if (result)
        {
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
