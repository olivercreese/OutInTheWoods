using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private GameObject GM;
    [SerializeField] AudioSource ambientSound;

    public void Awake()
    {
        GM = GameObject.Find("GameManager");
    }

    public void PlaySFX(AudioClip clip,AudioSource src)
    {
        if (!src.isPlaying)
            src.PlayOneShot(clip);
        Debug.Log(src.gameObject.name);
    }

    public void PlayAmbientSound(AudioClip clip)
    {
        ambientSound.clip = clip;
        ambientSound.Play();
    }
}
