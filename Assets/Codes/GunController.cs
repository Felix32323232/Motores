using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour, IWeapon
{
    [SerializeField] GunData GunData;
    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    int curAmmo;
    bool isReloading = false; 

    private float nextTimeToFire = 0f;

    void Awake()
    {
        curAmmo = GunData.maxAmmoCapacity;
    }

    public void Shoot(EnemyController enemy)
    {
        if (curAmmo <= 0 || isReloading || Time.time < nextTimeToFire) return;

        nextTimeToFire = Time.time + GunData.fireRate;

        Debug.Log("Shooting...");

        RaycastHit hit;
        Vector3 origin = barrel.transform.position;
        Vector3 direction = user.transform.forward;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, GunData.range))
        {
            endPoint = hit.point;

            EnemyController enemyHit = hit.transform.GetComponent<EnemyController>();
            if (enemyHit != null)
            {
                enemyHit.GetDamaged(GunData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * GunData.range;
        }

        curAmmo--;
        Debug.Log("Ammo: " + curAmmo);

        if (curAmmo <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (isReloading) return;

        StartCoroutine(WaitReloadTime());
    }

    public float GetRange() { return GunData.range; }
    public void SwitchWeapon() { }

    IEnumerator WaitReloadTime()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(GunData.reloadTime);

        curAmmo = GunData.maxAmmoCapacity;
        isReloading = false;
        Debug.Log("Reloaded");
    }
}