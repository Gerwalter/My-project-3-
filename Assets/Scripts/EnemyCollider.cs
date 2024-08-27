using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public GameObject sword;
    public GameObject particle;
    public float minY = 0.0f;  // Valor mínimo de Y para las partículas
    public float maxY = 2.0f;  // Valor máximo de Y para las partículas

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es la espada
        if (other.gameObject == sword)
        {
            // print("Sword hit the enemy");

            // Genera un valor aleatorio para la posición Y de las partículas
            float randomY = Mathf.Round(Random.Range(minY, maxY) * 100f) / 100f;
            print(randomY);

            // Instancia las partículas en la posición del objeto que fue golpeado con una Y aleatoria
            Instantiate(particle, new Vector3(other.transform.position.x, randomY, other.transform.position.z), other.transform.rotation);
        }
    }
}
