using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    List<GameObject> animals = new List<GameObject>();
    List<GameObject> monsters = new List<GameObject>();

    [SerializeField] GameObject blacksheepPrefab;
    [SerializeField] GameObject whitesheepPrefab;
    [SerializeField] GameObject goatPrefab;
    [SerializeField] GameObject MalusAranea;
    [SerializeField] GameObject InmundaFormica;
    [SerializeField] LightingManager LM;

    Terrain terrain;

    private bool isMonsterSpawned;
    private bool isAnimalSpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        isMonsterSpawned = false;
        for (int i = 0; i < 10; i++)
        {
            int random = Random.Range(1, 4);
            GameObject animalPrefab = null;
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
            animals.Add(animalPrefab);
        }
        isAnimalSpawned = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (LM.TimeOfDay > 20 && !isMonsterSpawned)
        {
            SpawnMonsters();
        }

        if (LM.TimeOfDay > 7 && LM.TimeOfDay < 20 && !isAnimalSpawned)
        {
            SpawnAnimals();
        }
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            int random = Random.Range(1, 3);
            GameObject monsterPrefab = null;
            if (animals[i] == null) continue;
            switch (random)
            {
                case 1:
                    monsterPrefab = Instantiate(MalusAranea, animals[i].transform.position, Quaternion.identity);
                    break;
                case 2:
                    monsterPrefab = Instantiate(InmundaFormica, animals[i].transform.position, Quaternion.identity);
                    break;
            }
            animals[i].GetComponent<Entity>().Die();
            monsters.Add(monsterPrefab);
        }
        animals.Clear();
        isMonsterSpawned = true;
        isAnimalSpawned = false;
    }

    void SpawnAnimals()
    {
        for (int i = 0; i < monsters.Count ; i++)
        {
            int random = Random.Range(1, 4);
            GameObject animalPrefab = null;
            if (monsters[i] == null) continue;
            switch (random)
            {
                case 1:
                    animalPrefab = Instantiate(whitesheepPrefab, monsters[i].transform.position, Quaternion.identity);
                    break;
                case 2:
                    animalPrefab = Instantiate(blacksheepPrefab, monsters[i].transform.position, Quaternion.identity);
                    break;
                case 3:
                    animalPrefab = Instantiate(goatPrefab, monsters[i].transform.position, Quaternion.identity);
                    break;
                default:
                    animalPrefab = Instantiate(whitesheepPrefab, monsters[i].transform.position, Quaternion.identity);
                    break;
            }
            monsters[i].GetComponent<Entity>().Die();
            animals.Add(animalPrefab);
        }
        monsters.Clear();
        isMonsterSpawned = false;
        isAnimalSpawned = true;
    }


}
