using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IEnemyTypeBehavior : MonoBehaviour
{
    public enum EnemyClass
    {
        Normal,
        Healer,
        Shooter,

    }


    [SerializeField] private NavMeshAgent agent;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform shootPoint;
    [SerializeField] public float shootCooldown = 2.0f;
    public float lastShootTime;

    [SerializeField] public EnemyClass enemyClass;

    public bool isNormal;
    public bool isHealer;
    public bool isShooter;

    protected virtual void Awake()
    {
        SetEnemyTypeBooleans();
    }

    public void SetEnemyTypeBooleans()
    {
       // agent.isStopped = true;
        isNormal = enemyClass == EnemyClass.Normal;
        isHealer = enemyClass == EnemyClass.Healer;
        isShooter = enemyClass == EnemyClass.Shooter;
    }

}
