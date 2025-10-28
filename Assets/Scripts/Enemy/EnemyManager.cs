using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] public float Enemycount;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Buscar todos los enemigos que existan en la escena al inicio
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in foundEnemies)
        {
            RegisterEnemy(e);
        }
    }


    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
}
