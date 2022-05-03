using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] SpacePlatformMovement lastPlatform;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLastPlatform(SpacePlatformMovement currentPlatform)
    {
        lastPlatform = currentPlatform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = lastPlatform.GetPosToRespawn();
        }
    }
}
