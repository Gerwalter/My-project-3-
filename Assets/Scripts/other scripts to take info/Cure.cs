using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int cure = 1;


    private void Start()
    {
        _player = GameManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        _player.Health(cure);
        Destroy(gameObject);
    }
}
