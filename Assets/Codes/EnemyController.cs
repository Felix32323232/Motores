using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject target;
    [SerializeField] EnemyData1 enemyData;

    float maxHealth;
    float currentHealth;

    void Start()
    {
        maxHealth = enemyData.GetMaxHealth();
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
    }
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target)
        {
            Move();
        }


    }
    void Move()
    {
        agent.destination = target.transform.position;
    }
    public void GetDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
