using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LootData
{
    public int gold;
}

[Serializable]
public class LootManager : MonoBehaviour
{

    #region Singleton
    public static LootManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeLootData();
    }
    #endregion

    private Dictionary<EnemyType, LootData> _enemyLoot = new Dictionary<EnemyType, LootData>();


    private void InitializeLootData()
    {
        _enemyLoot.Add(EnemyType.MELEE, new LootData { gold = 50 });
        _enemyLoot.Add(EnemyType.RANGE, new LootData { gold = 45 });
        _enemyLoot.Add(EnemyType.TANK, new LootData { gold = 25 });
        _enemyLoot.Add(EnemyType.BOSS, new LootData { gold = 100 });
    }

    public LootData GetLoot(EnemyType enemyType)
    {
        if (_enemyLoot.TryGetValue(enemyType, out var lootData))
        {
            return lootData;
        }

        return new LootData { gold = 0 };
    }
}
