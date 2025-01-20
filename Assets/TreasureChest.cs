using NUnit.Framework.Internal;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject lid;
    [SerializeField] GameObject treasure;
    private bool lidInPos=false;
    private bool playerInPos=false;
    private float lidRotation;
    

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            playerInPos = true;
            Debug.Log("inPos");
        }
    }
    private void Update()
    {
        if (playerInPos)
        {
            //lidRotation -= 1;
            //lidRotation = Mathf.Clamp(lidRotation, -90, 0);
            //lid.transform.localRotation = Quaternion.Euler(lidRotation, 0,0);
            Debug.Log(lid.transform.localRotation.x);
        }

        if (lid.transform.localRotation.x <= -90)
        {
            lidInPos = true;
        }
        else lidInPos = false;


        if (lidInPos)
        {
            treasure.SetActive(false);
        }
    }
}
