using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FireBall : BossAttacks
{
    public HP _target;
    public float speed = 10f; // Velocidad del movimiento
    private bool shouldMove = false; // Controla si el FireBall debe moverse
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayerMask;


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

    IEnumerator Charge()
    {
        yield return new WaitForSeconds(.2f);
        // Activa el movimiento después de cargar
        shouldMove = true;

        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
                // Verificamos si el objeto que colisiona está en la capa del jugador
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            // Solo aplicamos daño si el objeto está en la capa del jugador
            _target.ReciveDamage(damage);
        }
    }
}
