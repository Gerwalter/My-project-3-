using UnityEngine;

public interface IEnemy
{
    void Spawn(Vector3 position, AttackSlotType slotType);
    void Despawn();
}