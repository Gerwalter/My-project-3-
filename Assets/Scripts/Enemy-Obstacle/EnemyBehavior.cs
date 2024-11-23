using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemigo))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Shooting Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootRange = 10.0f;
    [SerializeField] private float projectileSpeed = 20.0f;
    [SerializeField] private bool shooting;

    private Enemigo enemigo; // Referencia al script principal
    private Transform target; // El objetivo del enemigo
    private float lastShootTime;

    private void Start()
    {
        enemigo = GetComponent<Enemigo>();
        target = GameManager.Instance.Player.gameObject.transform;

        if (target == null)
        {
            Debug.LogError("No se encontró al jugador en la escena.");
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // Solo dispara si el jugador está dentro del rango de disparo
        if (distanceToPlayer <= shootRange)
        {
            ShootAtPlayer(target);
            lastShootTime = Time.time;
        }
    }

    public float timeBetweenShots = 1f; // Tiempo entre disparos (en segundos)

    private float lastShotTime = 0f;
    public void ShootAtPlayer(Transform target)
    {
        // Verifica si el tiempo de recarga ha pasado
        if (shooting && Time.time - lastShotTime >= timeBetweenShots)
        {
            enemigo._anim.SetBool("isMoving", false);
            if (projectilePrefab == null || shootPoint == null)
            {
                Debug.LogError("Faltan referencias de disparo en EnemyBehavior.");
                return;
            }

            // Dispara el proyectil
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Vector3 directionToPlayer = (target.position - shootPoint.position).normalized;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * projectileSpeed;
            }

            Destroy(projectile, 5.0f); // Eliminar el proyectil después de 5 segundos

            // Actualizar el tiempo del último disparo
            lastShotTime = Time.time;
        }
    }

    public void StopShooting()
    {
        shooting = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar un rango de disparo en la vista de escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
