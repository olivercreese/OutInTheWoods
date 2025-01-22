using UnityEngine;
using UnityEngine.Audio;
//Audio Manager class
//This class is responsible for managing the audio in the game
//It plays the day and night time loops and the morning and night sounds by providing the necessary functions
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
        ambientSound.loop = true;
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
    }

    public void PlayNightTimeLoop()
    {
        ambientSound.clip = NightTimeLooping;
        ambientSound.Play();
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
