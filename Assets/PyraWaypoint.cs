using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PyraWaypoint : MonoBehaviour
{
    [SerializeField] GameObject wayPointGroup;
    [SerializeField] Image pyraWayPoint;
    [SerializeField] TextMeshProUGUI distanceText;
    GameObject player;
    float minX, minY, maxX, maxY;
    
    void Start()
    {
        minX = pyraWayPoint.GetPixelAdjustedRect().width / 2;
        Debug.Log(pyraWayPoint.GetPixelAdjustedRect().width / 2);
        maxX = Screen.width - minX;

        minY = pyraWayPoint.GetPixelAdjustedRect().height / 2;
        Debug.Log(pyraWayPoint.GetPixelAdjustedRect().height / 2);
        maxY = Screen.height - minY;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if(Vector3.Distance(player.transform.position, transform.position) >= 25f)
        {
            wayPointGroup.SetActive(true);
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);

            if(Vector3.Dot(player.transform.position - transform.position, Camera.main.transform.forward) > 0)
            {
                if(pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
            }
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            wayPointGroup.transform.position = pos;
            distanceText.text = ((int)Vector3.Distance(player.transform.position, transform.position)).ToString() + "m";
        }
        else
        {
            wayPointGroup.SetActive(false);
        }
    }
}
