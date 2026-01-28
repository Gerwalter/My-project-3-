using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;           // Velocidad de movimiento
    public float stopDistance = 1.5f;  // Distancia mínima para detenerse (ej. atacar)
    public Animator anim;
    public Transform player;
    public Enemy enemy;
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
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (player == null) return;
        if (enemy == null) return;

        if (enemy.CurrentState != EnemyStates.Chasing &&
            enemy.CurrentState != EnemyStates.WaitingForAttack)
        {
            anim.SetFloat("Move", 0f);
            return;
        }
        // Calcula la distancia al jugador
        float distance = Vector3.Distance(transform.position, player.position);

        // Si está más lejos que la distancia mínima, se mueve hacia el jugador
        if (distance > stopDistance)
        {
            speed = 3;
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Opcional: que mire hacia el jugador
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            speed = 0f;
        }

        anim.SetFloat("Move", speed > 0.1f ? 1f : 0f);
    }
}
