using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : BossAttacks
{
    public float speed = 5f;

    private void Update()
    {
        //prueba();
    }
    private void Awake()
    {
        ExecuteAttack();
    }
    public override void ExecuteAttack()
    {
        StartCoroutine(Charge());
    }

    IEnumerator Charge()
    {
        yield return new WaitForSeconds(.5f);
        print("A");
    }
}
