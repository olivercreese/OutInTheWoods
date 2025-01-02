using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    List<GameObject> animals = new List<GameObject>();

    [SerializeField] GameObject sheepPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject sheep = Instantiate(sheepPrefab, new Vector3(Random.Range(100, 400), 12, Random.Range(100, 400)), Quaternion.identity);
            //sheep.GetComponent<Sheep>().player = GameObject.Find("Player");
            animals.Add(sheep);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
