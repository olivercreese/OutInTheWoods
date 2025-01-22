using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] Transform[] Points; // array of waypoints for the helicopter to follow
    [SerializeField] float speed = 1.0f;
    [SerializeField] AudioClip heliSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameManager GM;
    private AudioManager audioManager;
    private int pointsIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {

        FollowWaypoints(); 
        audioManager.PlaySFX(heliSound, audioSource); // play the helicopter sound
    }

    void FollowWaypoints()
    {

        if (pointsIndex <= Points.Length - 1) // if the points index is less than the length of the points array
        {
            transform.position = Vector3.MoveTowards(transform.position, Points[pointsIndex].transform.position, speed * Time.deltaTime); // move the helicopter towards the next waypoint

            if (transform.position == Points[pointsIndex].transform.position) // if the helicopter reaches the waypoint
            {
                pointsIndex += 1; // move to the next waypoint
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GM.GameWon = true; // set the game won bool to true when the player reaches the helicopter
        }
    }
}
