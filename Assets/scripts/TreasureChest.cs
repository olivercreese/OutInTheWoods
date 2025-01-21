using NUnit.Framework.Internal;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject treasure;
    [SerializeField] GameManager GM;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            treasure.SetActive(false);
            GM.TreasureCount++;
        }
    }

}
