using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    InputAction m_interactAction;
    InputAction[] m_switchWeaponActions;
    IWeapon currentWeapon;
    ActionMap input;
    public int life = 100;
    public TextMeshProUGUI eliminadosText;
    public TextMeshProUGUI eliminadosMenuText;
    [SerializeField] GameObject Muerte;
    [SerializeField] GameObject Menu;

    [SerializeField] GameObject[] Weapons = new GameObject[3];

    public bool hit;
    public float invencibleTime = 2f;

    private bool cambiandoArmaManualmente = false;
    public enum TargetType
    { none, enemy, position }

    [SerializeField] LayerMask mask;

    Vector3 MousePosition = new Vector3();
    Target target = new Target(TargetType.none, new RaycastHit());

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new ActionMap();
        input.Main.Enable();
        m_interactAction = input.Main.Interact;

        input.Main.Gun1.performed += ctx => { cambiandoArmaManualmente = true; SwitchWeapon(0); };
        input.Main.Gun2.performed += ctx => { cambiandoArmaManualmente = true; SwitchWeapon(1); };
        input.Main.Gun3.performed += ctx => { cambiandoArmaManualmente = true; SwitchWeapon(2); };
    }

    void Start()
    {

        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i] != null)
            {
                Weapons[i].SetActive(i == 0); 
            }
        }

        if (Weapons[0] != null)
        {
            currentWeapon = Weapons[0].GetComponent<IWeapon>();
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Main.Disable();
        }
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

        if (m_interactAction.WasPressedThisFrame())
        {
            Move();
        }

        if (target.Type == TargetType.enemy && target.Hit.transform != null)
        {
            float distance = Vector3.Distance(transform.position, target.Hit.transform.position);

            if (currentWeapon != null && distance <= currentWeapon.GetRange())
            {
                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                }

                Vector3 lookTarget = new Vector3(target.Hit.transform.position.x, transform.position.y, target.Hit.transform.position.z);
                transform.LookAt(lookTarget);

                EnemyController enemy = target.Hit.transform.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    currentWeapon.Shoot(enemy);
                }
                else
                {
                    target = new Target(TargetType.none, new RaycastHit());
                }
            }
            else
            {

                if (target.Hit.transform != null)
                {
                    agent.isStopped = false;

                    if (Vector3.Distance(agent.destination, target.Hit.transform.position) > 0.2f)
                    {
                        agent.destination = target.Hit.transform.position;
                    }
                }
            }
        }

        if (life <= 0)
        {
            Die();
        }

        if(hit)
        {
            invencibleTime -= Time.deltaTime;
            if (invencibleTime <= 0f)
            {
                hit = false;
                invencibleTime = 2f; 
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if(hit) return;

        Debug.Log("Collided with: " + collision.gameObject.name);

        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

        if (collision.gameObject.CompareTag("Enemy") && enemy != null)
        {
            GetDamaged(enemy.enemyData.GetDamage());
        }
        hit = true;
    }

    void Move()
    {
        MousePosition = Mouse.current.position.value;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition), out hit, 100, mask))
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
        if (currentWeapon != null) Gizmos.DrawWireSphere(transform.position, currentWeapon.GetRange());
    }

    void GetDamaged(float damage)
    {
        life -= (int)damage;
        Debug.Log("Vida Restante:  " + life);
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= Weapons.Length) return;
        if (Weapons[weaponIndex] == null) return;

        if (!cambiandoArmaManualmente)
        {
            return; 
        }

        currentWeapon = Weapons[weaponIndex].GetComponent<IWeapon>();

        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i] != null)
            {
                Weapons[i].SetActive(i == weaponIndex);
            }
        }
      cambiandoArmaManualmente = false;
    }

    void Die()
    {
        Destroy(this.gameObject);
        Show(Muerte);
        int eliminadosFinal = EnemyController.eliminados;
        eliminadosText.text = "Eliminaciones totales: " + eliminadosFinal.ToString();
    }

    public void Show(GameObject Menu)
    {
        Menu.SetActive(true);
        Time.timeScale = 0f;
        int eliminadosFinal = EnemyController.eliminados;
        eliminadosMenuText.text = "Eliminaciones totales: " + eliminadosFinal.ToString();
    }
    public void Hide(GameObject Menu)
    {
        Menu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        life = 100;
    }
    public void ReturnToMenu()
    {
        EnemyController.eliminados = 0;
        SceneManager.LoadScene(0);
    }
}