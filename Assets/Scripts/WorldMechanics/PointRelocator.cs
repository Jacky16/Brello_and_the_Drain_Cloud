using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRelocator : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    void Start()
    { 
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 100000, layer);

        transform.position = new Vector3(hit.point.x, hit.point.y+1.2f, hit.point.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up);
    }
}
