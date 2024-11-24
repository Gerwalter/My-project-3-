using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    public virtual void ExecuteAttack(Transform target)
    {
        Debug.Log("Executing base attack logic.");
    }

    public virtual IEnumerator ExecuteAttacks (Transform target)
    {
        yield return new WaitForSeconds(0) ;
        Debug.Log("Executing base attack logic.");
    }
}
