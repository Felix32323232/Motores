using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData1", menuName = "Scriptable Objects/EnemyData1")]

public class EnemyData1 : ScriptableObject
{
    [SerializeField] float MAXhealth = 100f;
    [SerializeField] float damage = 5f;

    public float GetMaxHealth()
    {
        return MAXhealth;
    }
    public float GetDamage()
    {
        return damage;
    }
}
