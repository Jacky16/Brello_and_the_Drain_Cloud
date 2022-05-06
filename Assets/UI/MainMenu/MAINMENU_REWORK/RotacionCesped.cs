using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionCesped : MonoBehaviour
{
    public float GrassSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * GrassSpeed, 0, 0, Space.Self);
    }
}
