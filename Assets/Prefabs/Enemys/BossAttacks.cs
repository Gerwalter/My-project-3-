using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    public virtual void ExecuteAttack()
    {
        Debug.Log("Executing base attack logic.");
    }
}
