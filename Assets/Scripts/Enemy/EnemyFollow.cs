using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;           // Velocidad de movimiento
    public float stopDistance = 1.5f;  // Distancia mínima para detenerse (ej. atacar)

    public Transform player;

    void Start()
    {
        // Busca al objeto con tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcula la distancia al jugador
        float distance = Vector3.Distance(transform.position, player.position);

        // Si está más lejos que la distancia mínima, se mueve hacia el jugador
        if (distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Opcional: que mire hacia el jugador
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            // Aquí podrías poner la lógica de ataque
            Debug.Log("Atacando al jugador!");
        }
    }
}
