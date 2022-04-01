using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrelloOpenManager : MonoBehaviour
{
    [SerializeField] private GameObject openBrelloPrefab;
    [SerializeField] private GameObject closedBrelloPrefab;
    private bool isOpened;

    private void Start()
    {
        SetOpen(false);
    }

    public void SetOpen(bool _value)
    {
        isOpened = _value;
        openBrelloPrefab.SetActive(_value);
        closedBrelloPrefab.SetActive(!_value);
    }

    public bool GetIsOpen()
    {
        return isOpened;
    }
}