using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory
{
    private Dictionary<string, IObjectPool> pools = new Dictionary<string, IObjectPool>();
    public static EnemyFactory Instance { get; private set; }
    public void RegisterPool(string key, IObjectPool pool)
    {
        pools[key] = pool;
    }

    public IEnemy Create(string key, Vector3 position)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            return pool.Get(position);
        }

        Debug.LogError($"No existe un pool para '{key}'");
        return null;
    }

    public void Recycle(string key, IEnemy enemy)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            pool.Return(enemy);
        }
    }
}
