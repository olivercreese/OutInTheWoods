using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] NewPlayerController playerScript;
    [SerializeField] GameObject pistol;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerScript.PistolSwap();
            pistol.SetActive(false);
        }
    }
}
