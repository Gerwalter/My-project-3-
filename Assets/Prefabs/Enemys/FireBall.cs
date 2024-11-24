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

    public override IEnumerator ExecuteAttacks(Transform target)
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
        yield return new WaitForSeconds(.2f);
        print("a");
    }

    void prueba()
    {
      // StartCoroutine(Prueba());
        print("a");
    }

    IEnumerator Prueba()
    {
        yield return new WaitForSeconds(1);

    }
}
