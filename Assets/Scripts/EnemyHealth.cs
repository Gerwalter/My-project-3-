using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject sword;
    public GameObject Enemy;
    public float health;
    public float maxHealth;

    private void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es la espada
        if (other.gameObject == sword)
        {
            health -= 1;

            if (health <= 0)
                Destroy(Enemy);
        }
    }
}
