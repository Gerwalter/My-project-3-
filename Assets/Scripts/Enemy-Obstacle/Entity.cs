
using UnityEngine;

public class Entity : HP
{
    public enum EnemyClass
    {
        Normal,
        Healer,
        Shielder,
        Shooter,
    }


    [SerializeField] private GameObject shieldPrefab;
    protected GameObject _shieldInstance;
    [SerializeField] protected int _shieldLife = 50;

    // Booleanos para cada tipo de enemigo
    [SerializeField] protected bool _isNormal;
    [SerializeField] protected bool _isHealer;
    [SerializeField] protected bool _isShielder;
    [SerializeField] protected bool _isShooter;


    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected float shootCooldown = 2.0f;
    protected float lastShootTime;
    [SerializeField] public EnemyClass _enemyClass;


    private void Awake()
    {
        SetEnemyTypeBooleans();

        if (_shieldInstance == null && _isShielder)
        {
            _shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            _shieldInstance.transform.SetParent(transform);
            _shieldInstance.layer = gameObject.layer;
        }
    }

    private void SetEnemyTypeBooleans()
    {
        _isNormal = _enemyClass == EnemyClass.Normal;
        _isHealer = _enemyClass == EnemyClass.Healer;
        _isShielder = _enemyClass == EnemyClass.Shielder;
        _isShooter = _enemyClass == EnemyClass.Shooter;
    }

}

