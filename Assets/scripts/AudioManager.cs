using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private GameObject GM;
    [SerializeField] AudioSource ambientSound;
    [SerializeField] AudioClip DayTimeLooping;
    [SerializeField] AudioClip NightTimeLooping;
    [SerializeField] AudioClip MorningSound;
    [SerializeField] AudioClip NightSound;
    public AudioSource playerSFX;

    public void Awake()
    {
        GM = GameObject.Find("GameManager");
        PlayDayTimeLoop();
    }

    public void PlaySFX(AudioClip clip,AudioSource src)
    {
        if (!src.isPlaying)
            src.PlayOneShot(clip);
    }

    public void PlayDayTimeLoop()
    {
        ambientSound.clip = DayTimeLooping;
        ambientSound.Play();
        ambientSound.loop = true;
    }

    public void PlayNightTimeLoop()
    {
        ambientSound.clip = NightTimeLooping;
        ambientSound.Play();
        ambientSound.loop = true;
    }

    public void OnNightTime()
    {
        PlaySFX(NightSound,playerSFX);
    }

    public void OnMorning()
    {
        PlaySFX(MorningSound,playerSFX);
    }
}
