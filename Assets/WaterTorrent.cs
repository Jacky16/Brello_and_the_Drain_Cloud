using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTorrent : MonoBehaviour
{
    [SerializeField] float torrentForce;
    [SerializeField] Transform initPoint;
    [SerializeField] Transform endPoint;
    private Vector3 torrentDir;

    private void Start()
    {
        torrentDir = endPoint.position - initPoint.position;
    }
    
    public Vector3 GetTorrentDir()
    {
        return torrentDir.normalized * torrentForce;
    }
}
