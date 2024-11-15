using UnityEngine;
using static IEnemyTypeBehavior;
using UnityEngine.AI;

public class HealerBehavior : EnemyBehavior<EnemyClass>
{
    public int healRadius;

    public override void ExecuteBehavior()
    {
        Debug.Log("Executing Healer behavior...");
        // Lógica de curación para aliados
    }

    private void OnDrawGizmos()
    {
        
    }
}
