using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public float damage;
    public float range;
    public float fireRate;
    public int maxAmmoCapacity;
    public float reloadTime;

}
