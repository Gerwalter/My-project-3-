using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy2 : Entity
{


    [SerializeField] private GameObject projectilePrefab; // Proyectil que disparará el enemigo
    [SerializeField] private Transform shootPoint;        // Punto de origen del proyectil
    [SerializeField] private float shootCooldown = 2.0f;  // Intervalo entre disparos
    private float lastShootTime;



    private void Update()
    {
        if (Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (_target.position - shootPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 20f;
        Debug.Log($"{name} ha disparado.");
    }


    // Color para el shoot distance
    //  Gizmos.color = Color.blue;
    // Gizmos.DrawWireSphere(transform.position, _shootDist);


}



