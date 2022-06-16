using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;


public class RainZone : MonoBehaviour
{
    struct Position
    {
        public Vector3 position;
        public float distance;
    }

    private List<Transform> tpPoints;
    private PyraHealth pyra;
    private bool pyraInZone;
    private List<Position> positions;
    private PyraProtection pyraProtection;
    Volume rainVolume;

    private void Awake()
    {
        rainVolume = GetComponent<Volume>();
    }
    void Start()
    {
        Init();
        GetAllPoints();    
    }
    private void Init()
    {
        pyraProtection = GameObject.FindGameObjectWithTag("Player").GetComponent<PyraProtection>();
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraHealth>();
        
        tpPoints = new List<Transform>();
        positions = new List<Position>();
        pyraInZone = false;
    }
    private void GetAllPoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            tpPoints.Add(transform.GetChild(i));
        }
    }
    void Update()
    {
        if (pyraInZone)
        {
            if (!pyra.GetIsProtected())
            {
                pyra.DoDamage(1);
            }
        } 
    }
    private void CheckForClosestPoint()
    {
        positions.Clear();
        foreach(Transform position in tpPoints)
        {
            Position pos;
            pos.position = position.position;
            pos.distance = Vector3.Distance(pyra.transform.position, position.position);
            positions.Add(pos);
        }

        positions.Sort(SortByDistance);
        pyra.SetClosestPoint(positions[0].position);
    }
    private int SortByDistance(Position p1, Position p2)
    {
        return p1.distance.CompareTo(p2.distance);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pyra"))
        {
            pyraInZone = true;
            CheckForClosestPoint();
        }
        else if (other.CompareTag("Player"))
        {
            pyraProtection.SetIsInRain(true);
            EnableRaindPostProcess();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pyra"))
        {
            pyraInZone = false;          
        }
        else if (other.CompareTag("Player"))
        {
            pyraProtection.SetIsInRain(false);
            DisableRainPostProcess();
        }
    }
    void EnableRaindPostProcess()
    {
        DOTween.To(() => rainVolume.weight, x => rainVolume.weight = x, 1, 1);

    }
    void DisableRainPostProcess()
    {
        DOTween.To(() => rainVolume.weight, x => rainVolume.weight = x, 0, 1);
    }
}
