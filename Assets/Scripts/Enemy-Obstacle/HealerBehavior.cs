using UnityEngine;
using static IEnemyTypeBehavior;
using UnityEngine.AI;

public class HealerBehavior : EnemyBehavior<EnemyClass>
{
    public int healRadius;

    public override void ExecuteBehavior()
    {
        Debug.Log("Executing Healer behavior...");
        // L�gica de curaci�n para aliados
    }

    private void OnDrawGizmos()
    {
        
    }
}
