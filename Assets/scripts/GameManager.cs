using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    List<GameObject> animals = new List<GameObject>();

    [SerializeField] GameObject blacksheepPrefab;
    [SerializeField] GameObject whitesheepPrefab;
    [SerializeField] GameObject goatPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            int random = Random.Range(1, 4);
            GameObject animalPrefab;
            switch (random)
            {
                case 1:
                    animalPrefab = Instantiate(whitesheepPrefab, new Vector3(Random.Range(100, 400), 12, Random.Range(100, 400)), Quaternion.identity);
                    break;
                case 2:
                    animalPrefab = Instantiate(blacksheepPrefab, new Vector3(Random.Range(100, 400), 12, Random.Range(100, 400)), Quaternion.identity);
                    break;
                case 3:
                    animalPrefab = Instantiate(goatPrefab, new Vector3(Random.Range(100, 400), 12, Random.Range(100, 400)), Quaternion.identity);
                    break;
                default:
                    animalPrefab = Instantiate(whitesheepPrefab, new Vector3(Random.Range(100, 400), 12, Random.Range(100, 400)), Quaternion.identity);
                    break;
            }
            //sheep.GetComponent<Sheep>().player = GameObject.Find("Player");
            animals.Add(animalPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
