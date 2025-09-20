using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPersistent : MonoBehaviour
{
    [SerializeField] private string enemyID;

    private void Start()
    {
        // Si ya fue derrotado, lo destruimos al cargar la escena
        if (EnemyStateManager.Instance != null && EnemyStateManager.Instance.IsEnemyDefeated(enemyID))
        {
            gameObject.SetActive(false);
        }
    }

    public void DefeatEnemy()
    {
        if (EnemyStateManager.Instance != null)
        {
            EnemyStateManager.Instance.SetEnemyDefeated(enemyID);
        }

        gameObject.SetActive(false);
    }
}
