using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
public class GameManager : MonoBehaviour 
{

    List<GameObject> animals = new List<GameObject>(); //lists for storing the animals and monsters
    List<GameObject> monsters = new List<GameObject>();

    [SerializeField] GameObject blacksheepPrefab; //prefabs for the animals and monsters
    [SerializeField] GameObject whitesheepPrefab;
    [SerializeField] GameObject goatPrefab;
    [SerializeField] GameObject MalusAranea; 
    [SerializeField] GameObject InmundaFormica;
    [SerializeField] GameObject Helicopter; //helicopter prefab for the end of the game
    [SerializeField] TMP_Text TreasureText; //text for the treasure count
    [SerializeField] TMP_Text TimeOfDayText; //text for the time of day
    [SerializeField] Image bloodEffect; //image for the blood effect
    [SerializeField] Image FadeToBlack; //image for the fade to black effect
    public LightingManager LM; //lighting manager for direct access to the time of day
    private AudioManager audioManager; //audio manager for playing the night and day time loops
    private GameObject player;  

    public int TreasureCount;
    public bool GameWon;
    


    private bool isMonsterSpawned;
    private bool isAnimalSpawned;
   



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        isMonsterSpawned = false; // monster spawn flag set to false
        for (int i = 0; i < 25; i++) //loop for spawning the animals
        {
            int random = UnityEngine.Random.Range(1, 4); //random number for the animal type
            GameObject animalPrefab = null;
            switch (random) //switch case for the animal type spawns animals at random locations
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
        isAnimalSpawned = true; //animal spawn flag set to true when finished

        LM.TimeOfDay = 12.0f; 
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
            SpawnMonsters(); // spawns monsters after 8pm
        }

        if (LM.TimeOfDay > 7 && LM.TimeOfDay < 20 && !isAnimalSpawned)
        {
            SpawnAnimals(); //spawns animals after 7am
        }

        if (TreasureCount >= 6)
        {
            Helicopter.SetActive(true); //activates the helicopter when all the treasure is collected
        }
        
        UpdateTimeOfDay(); //updates the time of day text on the screen
        UpdatePlayerDamage(); //updates the blood effect on the screen
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

    public string FormatTime(float timeOfDay) //formats the time of day to a string
    {
        int hours = (int)timeOfDay; 
        int minutes = (int)((timeOfDay - hours) * 60); //calculates the minutes

        TimeSpan time = new TimeSpan(hours, minutes, 0); //creates a new timespan object with the hours and minutes 
        return time.ToString(@"hh\:mm"); //returns the time in the format hh:mm
    }
    public void OnDeath()
    {
        float fadealpha = FadeToBlack.color.a + Time.deltaTime / 2; //reducing the alpha value for the fade to black image
        FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadealpha);
        if (fadealpha >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0); //loads the main menu scene 
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
        audioManager.OnNightTime(); //plays the night time audio
        audioManager.PlayNightTimeLoop(); //plays the ambient night time loop
        for (int i = 0; i < animals.Count; i++) 
        {
            int random = UnityEngine.Random.Range(1, 3);
            GameObject monsterPrefab = null;
            if (animals[i] == null) continue;
            switch (random) //switch case for the monster type spawns monsters at the animal locations
            {
                case 1:
                    monsterPrefab = Instantiate(MalusAranea, animals[i].transform.position, Quaternion.identity);
                    break;
                case 2:
                    monsterPrefab = Instantiate(InmundaFormica, animals[i].transform.position, Quaternion.identity);
                    break;
            }
            animals[i].GetComponent<Entity>().Die(); //kills the animal
            monsters.Add(monsterPrefab); //adds the monster to the list
        }
        animals.Clear(); //clears the animal list
        isMonsterSpawned = true;
        isAnimalSpawned = false; //sets flags
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
