using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }

    public int health = 10;


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        // Avisar al EnemyManager que este enemigo murió
        EnemyManager.Instance.UnregisterEnemy(this);

        // Aquí puedes poner animación, efectos, etc.
        Destroy(gameObject);
    }

}
