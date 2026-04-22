using UnityEngine;

public interface IWeapon
{
    public void Shoot(EnemyController enemy);
    public void Reload();
    public float GetRange();
    public void SwitchWeapon();    

}