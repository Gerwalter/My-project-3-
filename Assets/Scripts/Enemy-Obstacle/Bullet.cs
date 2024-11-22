using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _shootDmg = 2;
    private void Start()
    {
        _player = GameManager.Instance.Player;
        // Si no choca con nada, se destruye después de 3 segundos
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el proyectil ha chocado con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            _player.ReciveDamage(_shootDmg);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("whatIsWall"))
        {
            Destroy(gameObject);
        }
    }
}
