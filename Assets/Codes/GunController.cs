using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour, IWeapon
{
    [SerializeField] GunData GunData;
    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    int curAmmo;
    bool CanShoot = true;

    void Awake()
    {
        curAmmo = GunData.maxAmmoCapacity;
    }
    public void Shoot(EnemyController enemy) 
    { 
        if (CanShoot == false) return;

        Debug.Log("Shooting...");

        RaycastHit hit;
        Vector3 origin = barrel.transform.position;
        Vector3 direction = user.transform.forward;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, GunData.range))
        {
            endPoint = hit.point;

            if (hit.transform.GetComponent<EnemyController>())
            {
                hit.transform.GetComponent<EnemyController>().GetDamaged(GunData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * GunData.range;
        }
        curAmmo--;

        Debug.Log("Ammo: " + curAmmo);
        
        if (curAmmo <= 0) Reload();



    }
    public void Reload()
    {
        CanShoot = false;
        StartCoroutine(WaitReloadTime());

    }
    public float GetRange() { return GunData.range; }
    public void SwitchWeapon() { }

    IEnumerator WaitFireRate()
    {
        CanShoot = false;
        yield return new WaitForSeconds(GunData.fireRate);
        CanShoot = true;
    }
    IEnumerator WaitReloadTime()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(GunData.reloadTime);
        curAmmo = GunData.maxAmmoCapacity;
        CanShoot = true;
        Debug.Log("Reloaded");
    }
}
