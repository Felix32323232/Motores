using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{

    NavMeshAgent agent;
    InputAction m_interactAction;
    InputAction[] m_switchWeaponActions;
    IWeapon currentWeapon;
    ActionMap input;

    [SerializeField] GameObject[] Weapons = new GameObject[3];

    public enum TargetType
    { none, enemy, position }

    [SerializeField] LayerMask mask;

    Vector3 MousePosition = new Vector3();
    Target target = new Target(TargetType.none,new RaycastHit());




    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new ActionMap();
        input.Main.Enable();
        m_interactAction = input.Main.Interact;
        m_switchWeaponActions = new InputAction[3] { input.Main.Gun1, input.Main.Gun2, input.Main.Gun3 };
    }
    void Start()
    {
        
        for (int i = 0; i < Weapons.Length; i++)
        {
                        if (i == 0) currentWeapon = Weapons[i].GetComponent<IWeapon>();
                        else
                        {
                             Weapons[i].SetActive(false);
                        }
        }
        currentWeapon = Weapons[0].GetComponent<IWeapon>();
        Debug.Log(Weapons[0].GetComponent<IWeapon>());
    }
    public struct Target
    {
        public Target(TargetType type, RaycastHit hit)
        {
            Type = type;
            Hit = hit;
        }
        public TargetType Type;
        public RaycastHit Hit;
    }

    void Update()
    {
        if(m_interactAction.WasPressedThisFrame())
        {
            Move();
            switch (target.Type)
            {
                case TargetType.enemy:
                    agent.destination = target.Hit.transform.position;
                    float distance = Vector3.Distance(transform.position, agent.destination);

                    Debug.Log(currentWeapon);

                    if (distance <= currentWeapon.GetRange())
                    {
                        Debug.Log("Shoot");
                        agent.isStopped = true;
                        transform.LookAt(agent.destination);
                        currentWeapon.Shoot(target.Hit.transform.GetComponent<EnemyController>());
                    }

                    break;
                case TargetType.position:
                default:
                    break;
            }

        }

    }
    void Move()
    {
        MousePosition = Mouse.current.position.value;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition), out hit, 100, mask))
        {
            string LayerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
            switch (LayerName)
            {
                case "Enemy":
                    target = new Target(TargetType.enemy, hit);
                    break;
                case "Walkable":
                    target = new Target(TargetType.position, hit);
                    break;
                default:
                    break;
            }
            // agent.destination = hit.point;           
            agent.destination = target.Hit.point;
            agent.isStopped = false;
        }
    }
    void OnDrawGizmos()
    {
        if (target.Type != TargetType.none)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        if(currentWeapon != null) Gizmos.DrawWireSphere(transform.position,currentWeapon.GetRange());
    }
    /*void Shoot()
    {
        canShoot = false;
        Debug.Log("Shoot");
        Debug.DrawLine(transform.position,target.Hit.transform.position,Color.yellow, 0.1f);
        StartCoroutine(ShootCooldown());
    }
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        canShoot = true;
    } */
}
