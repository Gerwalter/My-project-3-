
using UnityEngine;

public class Entity : HP
{
    public enum EnemyClass
    {
        Normal,
        Healer,
        Shooter,
        Shielder,
    }

    // Booleanos para cada tipo de enemigo
    [SerializeField] protected bool _isNormal;
    [SerializeField] protected bool _isHealer;
    [SerializeField] protected bool _isShielder;
    [SerializeField] protected bool _isShooter;


    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected GameObject _shieldInstance;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected float shootCooldown = 2.0f;
    [SerializeField] protected float _shieldLife = 2.0f;
    protected float lastShootTime;
    [SerializeField] public EnemyClass _enemyClass;

    


    private void Awake()
    {
        SetEnemyTypeBooleans();
    }

    private void SetEnemyTypeBooleans()
    {
        _isNormal = _enemyClass == EnemyClass.Normal;
        _isHealer = _enemyClass == EnemyClass.Healer;
        _isShooter = _enemyClass == EnemyClass.Shooter;
        _isShielder = _enemyClass == EnemyClass.Shielder;
    }
}

