using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        FollowWaypoints();  
    }

    void FollowWaypoints()
    {
        foreach (Transform waypoint in waypoints)
        {
            while (transform.position != waypoint.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed);
            }
        }
    }
}
