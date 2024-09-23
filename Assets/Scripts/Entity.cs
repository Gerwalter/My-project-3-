using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    public NavMeshAgent _agent;
    public Transform _target;


    [SerializeField] private bool isHealer;
    [SerializeField] public float _healDist = 5.0f;


    [SerializeField] private bool isShielder = false;
    private GameObject _shieldInstance;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] public float _shieldDist = 5.0f;


    [SerializeField] private bool isShooter = false;
    [SerializeField] public float _shootDist = 5.0f;

    private void Awake()
    {
        if (isShielder == true && _shieldInstance == null)
        {
            _shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            _shieldInstance.transform.SetParent(transform);
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
}



