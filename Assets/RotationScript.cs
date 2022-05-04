using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{

    [SerializeField] float rotationSpeed;
    [SerializeField] float movementSpeed;
    [SerializeField] float movementOffset;
    enum RotationType { POSITIVE, NEGATIVE, NONE };
    enum MovementType { UPWARDS, DOWNWARDS, NONE};
    [SerializeField] RotationType rotationType;
    [SerializeField] MovementType movementType;

    Vector3 initPos;
    void Start()
    {
        initPos = transform.position;
        rotationType = (RotationType)Random.Range(0, 2);
        movementType = (MovementType)Random.Range(0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(rotationType == RotationType.NEGATIVE)
        {
            transform.eulerAngles = Vector3.one * Time.time * rotationSpeed;
        }
        else if(rotationType == RotationType.POSITIVE)
        {
            transform.eulerAngles = Vector3.one * Time.time * -rotationSpeed;
        }

        if(movementType == MovementType.UPWARDS)
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * movementSpeed) * movementOffset + movementOffset, 0);
        }
        else if(movementType == MovementType.DOWNWARDS)
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * -movementSpeed) * movementOffset + movementOffset, 0);
        }
    }
}
