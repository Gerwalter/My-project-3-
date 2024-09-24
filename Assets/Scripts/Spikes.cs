using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void OnTriggerEnter(Collider other)
    {
        _player.ReciveDamage(4);
    }


    private void OnTriggerExit(Collider other)
    {
        _player.Health(1);
    }
}
