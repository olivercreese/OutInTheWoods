using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource player;
    [SerializeField] AudioSource AmbientSound;
    [SerializeField] AudioSource MalusAranea;
    [SerializeField] AudioSource InmundaFormica;
    [SerializeField] AudioSource Sheep;
    private GameObject GM;


    public AudioClip Playerwalk;
    public AudioClip PlayerRun;
    public AudioClip AmbientSound1;
    public AudioClip AmbientSound2;

    public void Awake()
    {
        GM = GameObject.Find("GameManager");
    }

    public void PlaySFX(AudioClip clip,AudioSource src)
    {
        if (!src.isPlaying)
            src.PlayOneShot(clip);
    }

    public void PlayAmbientSound(AudioClip clip)
    {
        AmbientSound.clip = clip;
        AmbientSound.Play();
    }
}
