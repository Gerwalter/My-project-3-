using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    protected Vector3 direction;

    // Configura la dirección del ataque
    public virtual void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public virtual void ExecuteAttack()
    {
        Debug.Log("Executing base attack logic.");
    }
}
