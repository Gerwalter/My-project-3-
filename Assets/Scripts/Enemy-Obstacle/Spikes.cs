using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        _player.ReciveDamage(damage);
    }
}
