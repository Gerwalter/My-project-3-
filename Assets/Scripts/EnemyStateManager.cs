using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance;

    // Guardamos el estado: ID del enemigo  derrotado o no
    private Dictionary<string, bool> defeatedEnemies = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Marca a un enemigo como derrotado
    /// </summary>
    public void SetEnemyDefeated(string enemyID)
    {
        if (!defeatedEnemies.ContainsKey(enemyID))
            defeatedEnemies.Add(enemyID, true);
        else
            defeatedEnemies[enemyID] = true;
    }

    /// <summary>
    /// Verifica si un enemigo ya fue derrotado
    /// </summary>
    public bool IsEnemyDefeated(string enemyID)
    {
        return defeatedEnemies.ContainsKey(enemyID) && defeatedEnemies[enemyID];
    }
}
