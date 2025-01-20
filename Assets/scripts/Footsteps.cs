using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footStepSounds;
    [SerializeField] private AudioSource audioSource;
    public bool isRunning;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            if (isRunning)
            {
                audioSource.PlayOneShot(footStepSounds[1]);
            }
            else
            {
                audioSource.PlayOneShot(footStepSounds[0]);
            }
        }
    }



}
