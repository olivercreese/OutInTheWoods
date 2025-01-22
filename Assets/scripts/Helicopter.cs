using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] Transform[] Points;
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
        audioManager.PlaySFX(heliSound, audioSource);
    }

    void FollowWaypoints()
    {

        if (pointsIndex <= Points.Length - 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, Points[pointsIndex].transform.position, speed * Time.deltaTime);

            if (transform.position == Points[pointsIndex].transform.position)
            {
                pointsIndex += 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GM.GameWon = true;
        }
    }
}
