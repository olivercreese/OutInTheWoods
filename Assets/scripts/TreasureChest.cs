using NUnit.Framework.Internal;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject treasure; // reference to the treasure inside the chest
    [SerializeField] AudioClip treasureSound;
    [SerializeField] AudioSource audioSource;
    private AudioManager audioManager;
    private GameManager GM;
    private bool treasureCollected;
    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        treasureCollected = false;  
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !treasureCollected) // if the player collides with the treasure chest and the treasure has not been collected
        {
            treasure.SetActive(false); // deactivate the treasure
            audioManager.PlaySFX(treasureSound, audioSource); // play the treasure sound
            GM.TreasureCount++;
            GM.UpdateTreasureText(); // update the treasure count
            treasureCollected = true;
        }
    }

}
