using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    List<GameObject> animals = new List<GameObject>();
    List<GameObject> monsters = new List<GameObject>();

    [SerializeField] GameObject blacksheepPrefab;
    [SerializeField] GameObject whitesheepPrefab;
    [SerializeField] GameObject goatPrefab;
    [SerializeField] GameObject MalusAranea;
    [SerializeField] GameObject InmundaFormica;
    [SerializeField] GameObject Helicopter;
    [SerializeField] TMP_Text TreasureText;
    [SerializeField] TMP_Text TimeOfDayText;
    [SerializeField] Image bloodEffect;
    [SerializeField] Image FadeToBlack;
    public LightingManager LM;
    private AudioManager audioManager;
    private GameObject player;

    public int TreasureCount;
    public bool GameWon;
    


    private bool isMonsterSpawned;
    private bool isAnimalSpawned;

    private int hours, minutes, seconds;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        isMonsterSpawned = false;
        for (int i = 0; i < 25; i++)
        {
            int random = UnityEngine.Random.Range(1, 4);
            GameObject animalPrefab = null;
            switch (random)
            {
                case 1:
                    animalPrefab = Instantiate(whitesheepPrefab, new Vector3(UnityEngine.Random.Range(-200, 800), 15, UnityEngine.Random.Range(-250, 600)), Quaternion.identity);
                    break;
                case 2:
                    animalPrefab = Instantiate(blacksheepPrefab, new Vector3(UnityEngine.Random.Range(-200, 800), 15, UnityEngine.Random.Range(-250, 600)), Quaternion.identity);
                    break;
                case 3:
                    animalPrefab = Instantiate(goatPrefab, new Vector3(UnityEngine.Random.Range(-200, 800), 15, UnityEngine.Random.Range(-250, 600)), Quaternion.identity);
                    break;
                default:
                    animalPrefab = Instantiate(whitesheepPrefab, new Vector3(UnityEngine.Random.Range(-200, 800), 15, UnityEngine.Random.Range(-250, 600)), Quaternion.identity);
                    break;
            }
            animals.Add(animalPrefab);
        }
        isAnimalSpawned = true;

        LM.TimeOfDay = 12.0f;
        hours = 12;
        player = GameObject.FindWithTag("Player");
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameWon)
        {
            OnWin();
        }
        if (LM.TimeOfDay > 20 && !isMonsterSpawned)
        {
            SpawnMonsters();
        }

        if (LM.TimeOfDay > 7 && LM.TimeOfDay < 20 && !isAnimalSpawned)
        {
            SpawnAnimals();
        }

        if (TreasureCount >= 6)
        {
            Helicopter.SetActive(true);
        }
        
        UpdateTimeOfDay();
        UpdatePlayerDamage();
    }

    public void UpdateTreasureText()
    {
        TreasureText.text = TreasureCount + "/6";
    }

    private void UpdateTimeOfDay()
    {
        TimeOfDayText.text = FormatTime(LM.TimeOfDay);
    }

    private void UpdatePlayerDamage()
    {
        float playerHealth = player.GetComponent<Entity>().currentHealth;

        switch (playerHealth)
        {
            case 100:
                bloodEffect.color = new Color(bloodEffect.color.r, bloodEffect.color.g, bloodEffect.color.b, 0.0f);
                break;
            case 75:
                bloodEffect.color = new Color(bloodEffect.color.r, bloodEffect.color.g, bloodEffect.color.b, 0.25f);
                break;
            case 50:
                bloodEffect.color = new Color(bloodEffect.color.r, bloodEffect.color.g, bloodEffect.color.b, 0.5f);
                break;
            case 25:
                bloodEffect.color = new Color(bloodEffect.color.r, bloodEffect.color.g, bloodEffect.color.b, 0.75f);
                break;
            case 0:
                bloodEffect.color = new Color(bloodEffect.color.r, bloodEffect.color.g, bloodEffect.color.b, 1.0f);
                OnDeath();
                break;
        }
    }

    public string FormatTime(float timeOfDay)
    {
        int hours = (int)timeOfDay;
        int minutes = (int)((timeOfDay - hours) * 60);

        TimeSpan time = new TimeSpan(hours, minutes, 0);
        return time.ToString(@"hh\:mm");
    }
    public void OnDeath()
    {
        float fadealpha = FadeToBlack.color.a + Time.deltaTime / 2;
        FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadealpha);
        if (fadealpha >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }


    private void OnWin()
    {
        float fadealpha = FadeToBlack.color.a + Time.deltaTime/5;
        FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadealpha);
        if (fadealpha >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    void SpawnMonsters()
    {
        audioManager.OnNightTime();
        audioManager.PlayNightTimeLoop();
        for (int i = 0; i < animals.Count; i++)
        {
            int random = UnityEngine.Random.Range(1, 3);
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
        audioManager.OnMorning();
        audioManager.PlayDayTimeLoop();
        for (int i = 0; i < monsters.Count; i++)
        {
            int random = UnityEngine.Random.Range(1, 4);
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
