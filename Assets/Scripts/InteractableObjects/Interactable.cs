using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float priority;
    [SerializeField] protected PyraAI pyra;
    virtual public void Interact() { }
}
