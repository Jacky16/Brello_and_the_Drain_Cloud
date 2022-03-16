using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrelloOpenManager : MonoBehaviour
{
    [SerializeField] private GameObject openBrelloPrefab;
    [SerializeField] private GameObject closedBrelloPrefab;

    private void Start()
    {
        CloseBrello();
    }

    public void OpenBrello()
    {
        closedBrelloPrefab.SetActive(false);
        openBrelloPrefab.SetActive(true);
    }

    public void CloseBrello()
    {
        closedBrelloPrefab.SetActive(true);
        openBrelloPrefab.SetActive(false);
    }
}