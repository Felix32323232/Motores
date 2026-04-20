using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject target;


    void Start()
    {
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

}
