using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxLife;
    [SerializeField] private float currentLife;

    public float GetLife
    {
        get { return currentLife; }
        set { currentLife = Mathf.Clamp(value, 0, maxLife); }
    }


    public void ReciveDamage(float damage)
    {
        // Reducimos la vida según el daño recibido.
        GetLife -= damage;

        // Verificamos si el jugador ha muerto.
        if (GetLife <= 0)
        {
            Die();
        }
    }

    public void Health(float amount)
    {
        // Aumentamos la vida del jugador al curarlo.
        GetLife += amount;
    }

    private void Die()
    {
        // Aquí se define lo que sucede cuando el jugador muere.
        Debug.Log("Player is dead!");
        // Puedes agregar más lógica, como desactivar al jugador o mostrar una pantalla de muerte.
    }
}
