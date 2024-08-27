using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public GameObject sword;
    public GameObject particle;
    public float minY = 0.0f;  // Valor m�nimo de Y para las part�culas
    public float maxY = 2.0f;  // Valor m�ximo de Y para las part�culas

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es la espada
        if (other.gameObject == sword)
        {
            // print("Sword hit the enemy");

            // Genera un valor aleatorio para la posici�n Y de las part�culas
            float randomY = Mathf.Round(Random.Range(minY, maxY) * 100f) / 100f;
            print(randomY);

            // Instancia las part�culas en la posici�n del objeto que fue golpeado con una Y aleatoria
            Instantiate(particle, new Vector3(other.transform.position.x, randomY, other.transform.position.z), other.transform.rotation);
        }
    }
}
