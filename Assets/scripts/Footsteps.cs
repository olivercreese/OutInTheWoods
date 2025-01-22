using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip footStepSound;
    [SerializeField] private AudioSource audioSource;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain") // if the feet collider collides with the terrain object
        {
           audioManager.PlaySFX(footStepSound,audioSource);
            
        }
    }
}
