using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image FadeToBlack;
    [SerializeField] AudioClip nightTimeLoop;
    [SerializeField] AudioClip dayTimeLoop;
    [SerializeField] AudioSource ambientSound;
    private LightingManager LM;
    private bool startPressed;
    //lightweight manager class that handles the menu screen and the audio 

    void Start()
    {
        LM = GameObject.FindWithTag("LightingManager").GetComponent<LightingManager>();
        startPressed = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (LM.TimeOfDay > 20) //if the time of day is greater than 20 play the night time loop
        {
            ambientSound.clip = nightTimeLoop;
            if (!ambientSound.isPlaying) 
            {
                ambientSound.Play();
            }
        }
        else if (LM.TimeOfDay > 6) //if the time of day is greater than 6 play the day time loop
        {
            ambientSound.clip = dayTimeLoop;
            if (!ambientSound.isPlaying)
            {
                ambientSound.Play();
            }
        }

        if (startPressed) //if the start button is pressed fade to black and load the game scene
        {
            float fadealpha = FadeToBlack.color.a + Time.deltaTime / 2;
            FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadealpha);
            if (fadealpha >= 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        }
    }
    public void OnPressExit()
    {
        Application.Quit(); //exit the application
    }


    public void OnPressStart()
    {
        startPressed = true; //set the start pressed bool to true when the start button is pressed
    }
}
