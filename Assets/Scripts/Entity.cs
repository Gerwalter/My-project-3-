using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    public NavMeshAgent _agent;
    public Transform _target;


    [SerializeField] private bool isHealer;
    [SerializeField] public float _healDist = 5.0f;


    [SerializeField] private bool isShielder = false;
    [SerializeField] private GameObject _shieldInstance;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] public float _shieldDist = 5.0f;


    [SerializeField] private bool isShooter = false;
    [SerializeField] public float _shootDist = 5.0f;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2.0f;
    private float lastShootTime;


    private void Awake()
    {
        if (_shieldInstance == null)
        {
            _shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            _shieldInstance.transform.SetParent(transform);
        }

        if (isShielder != true)
        {
            _shieldInstance.SetActive(false);
        }
    }
    private void Update()
    {
        if (isHealer)
        {
            Enemy nearbyAlly = FindAllyToHeal();
            if (nearbyAlly != null)
            {
                HealAlly(nearbyAlly);
                return;
            }
        }

        if (isShielder)
        {
            if (PlayerInShieldRange())
            {
                ActivateShield();
            }
            else
            {
                DeactivateShield();
            }
        }

        if (isShooter)
        {
            if (PlayerInShieldRange())
            {
                if (Time.time >= lastShootTime + shootCooldown)
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }
        }
    }

    private Enemy FindAllyToHeal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _healDist);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy != this && enemy.GetLife < enemy.maxLife)
            {
                return enemy;
            }
        }
        return null;
    }

    private void HealAlly(Enemy ally)
    {
        _agent.SetDestination(ally.transform.position);
        if (Vector3.Distance(transform.position, ally.transform.position) <= 1.0f)
        {
            ally.Health(10);
        }
    }
    private void DeactivateShield()
    {
        if (_shieldInstance != null)
        {
            _shieldInstance.SetActive(false);
        }
    }

    private bool PlayerInShieldRange()
    {
        return Vector3.Distance(transform.position, _target.position) <= _shieldDist;
    }

    private void ActivateShield()
    {

        _shieldInstance.SetActive(true);

    }

    private bool PlayerInShootRange()
    {
        return Vector3.Distance(transform.position, _target.position) <= _shootDist;
    }

    public void SetEntityType(Enemy.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case Enemy.EnemyType.Healer:
                isHealer = true;
                isShielder = false;
                isShooter = false;
                break;
            case Enemy.EnemyType.Shielder:
                isHealer = false;
                isShielder = true;
                isShooter = false;
                break;

            case Enemy.EnemyType.Shooter:
                isShielder = false;
                isHealer = false;
                isShooter = true;
                break;
            case Enemy.EnemyType.Normal:
            default:
                isHealer = false;
                isShielder = false;
                isShooter = false;
                break;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (_target.position - shootPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 20f;

        Destroy(projectile, 3f);
    }
}



