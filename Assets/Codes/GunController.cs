using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    int curAmmo;
    bool CanShoot = true;

    void Awake()
    {
        curAmmo = gunData.maxAmmoCapacity;
    }
    void Shoot(EnemyController enemy) 
    { 
        if (CanShoot == false) return;
        RaycastHit hit;
        Vector3 origin = barrel.transform.position;
        Vector3 direction = user.transform.forward;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, gunData.range))
        {
            endPoint = hit.point;

            if (hit.transform.GetComponent<EnemyController>())
            {
                hit.transform.GetComponent<EnemyController>().GetDamaged(gunData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * gunData.range;
        }
        curAmmo--;

        Debug.Log("Ammo: " + curAmmo);
        
        if (curAmmo <= 0) Reload();



    }
    void Reload()
    {
        CanShoot = false;
        StartCoroutine(WaitReloadTime());

    }
    float GetRange() { return 0f; }
    void SwitchWeapon() { }

    IEnumerator WaitFireRate()
    {
        CanShoot = false;
        yield return new WaitForSeconds(gunData.fireRate);
        CanShoot = true;
    }
    IEnumerator WaitReloadTime()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(gunData.reloadTime);
        curAmmo = gunData.maxAmmoCapacity;
        CanShoot = true;
        Debug.Log("Reloaded");
    }
}
