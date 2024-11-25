using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tornado : BossAttacks
{
    public Player _target;
    public float speed = 10f; // Velocidad del movimiento
    private bool shouldMove = false; // Controla si el FireBall debe moverse


    private void Awake()
    {
        _target = GameManager.Instance.Player;

        ExecuteAttack();
    }
    private void Update()
    {
        if (shouldMove)
        {
            // Mueve el FireBall hacia adelante en la dirección que está enfrentando
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    public override void ExecuteAttack()
    {
        StartCoroutine(Charge());
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == _target)
        {
                    print("A");
        }

    }
    IEnumerator Charge()
    {
        yield return new WaitForSeconds(.2f);
        // Activa el movimiento después de cargar
        shouldMove = true;

        Destroy(gameObject, 4f);
    }
}
