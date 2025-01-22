using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform gunTip;

    private int damage = 35;
    private float range = 100f;
    private int Ammo = 6;


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        Ammo--;
        RaycastHit hit;
        if (Physics.Raycast(gunTip.position, gunTip.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.tag == "Enemy")
            {
                hit.transform.GetComponent<Entity>().TakeDamage(damage);
            }
        }
    }
}
