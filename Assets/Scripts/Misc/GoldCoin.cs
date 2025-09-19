using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : Coin
{
    public int GoldAmmount;
    public float detectionRadius = 5f;   // Rango en el que detecta al jugador
    public float collectRadius = 2f;       // Rango en el que puede atacar
    public LayerMask playerLayer;
    private Transform targetPlayer;
    private void Start()
    {
        collectRadius = detectionRadius;
    }
    private void Update()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (players.Length > 0)
        {
            targetPlayer = players[0].transform;

            // Verificar si está lo suficientemente cerca para atacar
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance <= collectRadius)
            {
                Colect();
            }
        }
        else
        {
            targetPlayer = null;
        }
    }


public override void Colect()
    {
        EventManager.Trigger("IncreaseGold", GoldAmmount);
        Destroy(gameObject); // Destruye la moneda
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}
