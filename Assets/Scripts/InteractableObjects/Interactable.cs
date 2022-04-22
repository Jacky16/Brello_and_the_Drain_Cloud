using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float priority;
    protected PyraAI pyra;
    virtual public void Interact() { }

    virtual public void ResetInter() { }

    virtual protected void ResetAll() { }

    protected virtual void Start()
    {
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pyra"))
        {
            if (pyra.currentInteractuable == this)
            {
                Interact();
                pyra.isInteracting = true;
                pyra.agent.SetDestination(pyra.transform.position);
            }
        }
    }
}
