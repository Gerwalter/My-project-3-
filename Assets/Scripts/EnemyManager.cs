using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        // Singleton para acceso fácil
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

    /// <summary>
    /// Registrar enemigo en la lista
    /// </summary>
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// Eliminar enemigo de la lista
    /// </summary>
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            if (enemies.Count == 0)
            {
                // Cuando no quedan enemigos, victoria
                Debug.Log("Todos los enemigos derrotados, fin de la batalla.");
                FindObjectOfType<BattleEnd>()?.OnBattleWon();
            }
        }
    }
}
