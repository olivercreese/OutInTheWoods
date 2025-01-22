using NUnit.Framework.Internal;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject treasure;
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
        if (other.gameObject.tag == "Player" && !treasureCollected)
        {
            treasure.SetActive(false);
            audioManager.PlaySFX(treasureSound, audioSource);
            GM.TreasureCount++;
            GM.UpdateTreasureText();
            treasureCollected = true;
        }
    }

}
