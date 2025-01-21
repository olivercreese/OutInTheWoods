using NUnit.Framework.Internal;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject treasure;
    [SerializeField] AudioClip treasureSound;
    [SerializeField] AudioSource audioSource;
    private AudioManager audioManager;
    private GameManager GM;
    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            treasure.SetActive(false);
            audioManager.PlaySFX(treasureSound, audioSource);
            GM.TreasureCount++;
        }
    }

}
