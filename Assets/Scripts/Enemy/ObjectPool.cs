using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool where T : MonoBehaviour, IEnemy
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool: prefab es null. No puedo crear el pool.");
            return;
        }

        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public IEnemy Get(Vector3 position)
    {
        T obj;
        if (pool.Count > 0)
            obj = pool.Dequeue();
        else
            obj = GameObject.Instantiate(prefab, parent);

        obj.Spawn(position);
        return obj;
    }

    public void Return(IEnemy enemy)
    {
        if (enemy == null) return;
        enemy.Despawn();
        pool.Enqueue((T)enemy);
    }
}
