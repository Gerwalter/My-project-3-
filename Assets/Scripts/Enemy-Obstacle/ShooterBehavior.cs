using UnityEngine;
using static IEnemyTypeBehavior;

public class ShooterBehavior : EnemyBehavior<EnemyClass>
{
    public override void ExecuteBehavior()
    {
        if (EnemyClass == EnemyClass.Shooter)
        {
            Debug.Log("Shooting at the player...");
            // Lógica para disparar
        }
    }
}
