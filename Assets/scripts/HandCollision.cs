using UnityEngine;

public class HandCollision : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GetComponentInParent<MalusAranea>().currentState == MalusAranea.monsterState.Attacking)
        {
            Debug.Log("Player hit by hand");
        }
    }
}
