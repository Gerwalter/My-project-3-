
using UnityEngine;

public class Entity : HP
{
    public enum EnemyType
    {
        Normal,
        Healer,
        Shielder,
        Shooter,
    }

    [SerializeField] public EnemyType _enemyType;
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
        _isNormal = _enemyType == EnemyType.Normal;
        _isHealer = _enemyType == EnemyType.Healer;
        _isShielder = _enemyType == EnemyType.Shielder;
        _isShooter = _enemyType == EnemyType.Shooter;
    }

}

