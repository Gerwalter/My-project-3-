using UnityEngine;

public interface IObjectPool
{
    IEnemy Get(Vector3 position);
    void Return(IEnemy enemy);
}