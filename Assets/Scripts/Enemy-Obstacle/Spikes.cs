using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayerMask;

    private void Awake()
    {
        _player = GameManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {

        // Verificamos si el objeto que colisiona está en la capa del jugador
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            // Solo aplicamos daño si el objeto está en la capa del jugador
            _player.ReciveDamage(damage);
        }
    }
}
