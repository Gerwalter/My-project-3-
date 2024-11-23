using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class Boss : HP
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] public float _speed = 3.0f; // Velocidad del enemigo
    [SerializeField] public EnemyType enemyType;

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float liftForce = 10.0f;

    [SerializeField]
    private Transform _target;

    private void Start()
    {
        _target = GameManager.Instance.Player.gameObject.transform;

        if (!_target)
        {
            Debug.LogError("No hay target.");
        }
        else
        {
            Debug.Log($"Target : {_target.gameObject.name}.");
        }

        GetLife = maxLife;
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
            Attack();
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
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 2;

    private Ray _atkRay;
    private RaycastHit _atkHit;
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Player>(out Player player))
            {
                player.ReciveDamage(_atkDmg);
            }
        }
    }

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
        LootData loot = WaveManager.Instance.GetLoot(enemyType);
       
        if (FindObjectOfType<PlayerStats>() is PlayerStats playerStats)
        {
            playerStats.AddLoot(loot);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_atkRay);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkDist);
    }
}
