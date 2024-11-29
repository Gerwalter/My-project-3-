using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;

[System.Serializable]
public struct Attack
{
    public string Name;                  // Nombre del ataque
    public float Cooldown;               // Tiempo de reutilización del ataque
    public GameObject AttackPrefab;      // Prefab del ataque para instanciarlo

    public Attack(string name, float cooldown, GameObject attackPrefab)
    {
        Name = name;
        Cooldown = cooldown;
        AttackPrefab = attackPrefab;
    }
}

public class Boss : HP
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] public float _speed = 3.0f; // Velocidad del enemigo
    [SerializeField] public EnemyType enemyType;

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;
    [SerializeField] private bool isdead;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float liftForce = 10.0f;

    [SerializeField]
    private Transform _target;

    [SerializeField] private Attack[] attacks;

    private void Start()
    {
        _target = GameManager.Instance.Player.gameObject.transform;
        GetLife = maxLife;
    }

    private bool canAttack = true;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float yAxis;
    private void PerformRandomAttack()
    {
        if (canAttack)
        {
            // Selecciona un ataque aleatorio del arreglo
            int randomIndex = Random.Range(0, attacks.Length);
            Attack selectedAttack = attacks[randomIndex];
            Transform spawnPoint = GetRandomSpawnPoint();
            Vector3 FireSpawn = new Vector3(spawnPoint.position.x, yAxis, spawnPoint.position.z);
            // Instancia el prefab del ataque (si existe)
            if (selectedAttack.AttackPrefab != null)
            {
                if (selectedAttack.Name == "Fireball")
                {

                    // Ajusta la posición en y del spawnPoint a un valor fijo
                    spawnPoint.position = FireSpawn;
                }
                // Instancia el prefab y alinea su rotación con el forward del invocador
                GameObject attackInstance = Instantiate(selectedAttack.AttackPrefab,spawnPoint.position,Quaternion.LookRotation(transform.forward));

                // Si el prefab tiene un script que necesita configuración, lo hacemos aquí
                var attackComponent = attackInstance.GetComponent<BossAttacks>();
                if (attackComponent != null)
                {
                    attackComponent.SetDirection(transform.forward);
                }

               

            }

            StartCoroutine(AttackCooldownRoutine(selectedAttack.Cooldown));
            canAttack = false;
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex];
    }
    private IEnumerator AttackCooldownRoutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }

    public void ApplyLiftImpulse()
    {
        _groundCheckDistance = 0;
        rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        yield return new WaitForSeconds(1);
        _groundCheckDistance = 1.1f;
    }

    [SerializeField] private float _groundCheckDistance = 1.1f;

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
    }

    float distanceToPlayer = 0.0f;
    private void Update()
    {
        UpdateHealthBar();

        distanceToPlayer = Vector3.Distance(transform.position, _target.position);

        if (distanceToPlayer <= _chaseDist && distanceToPlayer > _atkDist)
        {
            MoveTowardsTarget();
        }
        else if (distanceToPlayer <= _atkDist)
        {
            PerformRandomAttack();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        direction.y = 0; // Evitar que el enemigo intente moverse en el eje Y

        transform.Translate(direction * _speed * Time.deltaTime, Space.World);

        // Actualizar la rotación para que mire al jugador
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _speed);
        _animator.SetBool("isMoving", true);
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private LayerMask _atkMask;


    [SerializeField] private ElementType weakness; // Tipo de debilidad del enemigo
    [SerializeField] private float elementalMultiplier = 2.0f;

    public void ReciveDamage(float dmg, ElementType damageType)
    {
        if (damageType == weakness)
        {
            dmg *= elementalMultiplier; // Aumenta el daño si coincide con la debilidad
        }
        GetLife -= dmg;
        _animator.SetTrigger("Hit");
        if (GetLife <= 0)
        {
            Die();
        }
        else
        {
            _bloodVFX.SendEvent("OnTakeDamage");
            //_animator.ResetTrigger("Hit");
        }
    }

    private void Die()
    {
        _animator.SetBool("isMoving", false);
        _animator.ResetTrigger("Punch");


        _animator.SetTrigger("Die");

        if (!isdead)
        {

            LootData loot = LootManager.Instance.GetLoot(enemyType);

            if (FindObjectOfType<PlayerStats>() is PlayerStats playerStats)
            {
                playerStats.AddLoot(loot);
            }
        }
        isdead = true;
        Destroy(gameObject, 2.5f);
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);
    }
}

