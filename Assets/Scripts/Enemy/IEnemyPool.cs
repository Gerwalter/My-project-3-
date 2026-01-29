using UnityEngine;

public interface IEnemyPool
{
    IEnemy Get(Vector3 position, AttackSlotType slotType);
    void Return(IEnemy enemy);
}