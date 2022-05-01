using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceRockMovement : MonoBehaviour
{
    Vector3 initPos;
    enum MovementType {UPWARDS, DOWNWARDS};
    MovementType movementType;
    float randomMovementSpeed;
    float randomMovementOffset;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        movementType = (MovementType)Random.Range(0, 2);
        randomMovementSpeed = Random.Range(0.3f, 0.7f);
        randomMovementOffset = Random.Range(0.75f, 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        if(movementType == MovementType.UPWARDS)
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * randomMovementSpeed) * randomMovementOffset + randomMovementOffset,0);
        }
        else
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * -randomMovementSpeed) * randomMovementOffset + randomMovementOffset,0);
        }
    }
}
