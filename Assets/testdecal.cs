using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testdecal : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        Physics.Raycast(transform.position, Vector3.left, out RaycastHit hit, layerMask);

        transform.position = hit.point;
        transform.forward = hit.normal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
