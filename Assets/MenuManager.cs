using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image FadeToBlack;
    [SerializeField] AudioClip nightTimeLoop;
    [SerializeField] AudioClip dayTimeLoop;
    [SerializeField] AudioSource ambientSound;
    private LightingManager LM;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LM = GameObject.FindWithTag("LightingManager").GetComponent<LightingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LM.TimeOfDay > 20)
        {
            ambientSound.clip = nightTimeLoop;
            if (!ambientSound.isPlaying)
            {
                ambientSound.Play();
            }
        }
        else if (LM.TimeOfDay > 6)
        {
            ambientSound.clip = dayTimeLoop;
            if (!ambientSound.isPlaying)
            {
                ambientSound.Play();
            }
        }
    }

    public void OnPressStart()
    {
        float fadealpha = FadeToBlack.color.a + Time.deltaTime / 2;
        FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadealpha);
        if (fadealpha >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
