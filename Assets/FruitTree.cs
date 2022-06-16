using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTree : Health{

    [SerializeField] float timeToRespawnFruits;
    [SerializeField] GameObject fruitPrefab;

    float timer;
    bool fruitDropped;
    int treeFruits;

    List<Fruit> fruitList;
    List<Transform> fruitSpawnerList;

    protected override void Start()
    {
        base.Start();
        fruitDropped = false;
        timer = 0f;

        fruitList = new List<Fruit>();
        fruitSpawnerList = new List<Transform>();

        treeFruits = transform.childCount;

        for (int i = 0; i < treeFruits; i++)
        {
            fruitSpawnerList.Add(transform.GetChild(i));
        }

        RespawnFruits();
    }

    protected override void onDamage()
    {
        DropFruits();
        Debug.Log("Entro");
    }

    void Update()
    {
        //if (fruitDropped)
        //{
        //    timer += Time.deltaTime;
        //    if(timer > timeToRespawnFruits)
        //    {
        //        RespawnFruits();
        //        timer = 0;
        //    }
        //}
    }

    private void DropFruits()
    {
        fruitDropped = true;

        foreach(Fruit fruit in fruitList)
        {
            fruit.Drop();
        }

        fruitList.Clear();
    }

    private void RespawnFruits()
    {
        fruitDropped = false;

        foreach(Transform posToSpawn in fruitSpawnerList)
        {
            fruitList.Add(Instantiate(fruitPrefab, posToSpawn).GetComponent<Fruit>());
        }
    }
}
