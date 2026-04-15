using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    InputAction m_interactAction;
    Vector3 MousePosition = new Vector3();
    ActionMap input;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new ActionMap();
        input.Main.Enable();
        m_interactAction = input.Main.Interact;
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(m_interactAction.WasPressedThisFrame())
        {
            Move();
        }
    }
    void Move()
    {
        MousePosition = Mouse.current.position.value;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition), out hit, 100))
        {
            agent.destination = hit.point;
        }
    }
}
