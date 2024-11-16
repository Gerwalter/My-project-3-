using UnityEngine;

public class ShootBehavior : IShootBehavior
{
    private GameObject projectilePrefab;
    private Transform shootPoint;
    private float projectileSpeed;

    public ShootBehavior(GameObject projectilePrefab, Transform shootPoint, float projectileSpeed)
    {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = shootPoint;
        this.projectileSpeed = projectileSpeed;
    }

    public void ShootAtTarget(Transform target)
    {
        if (target == null) return;

        // Crear el proyectil
        GameObject projectile = GameObject.Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        // Calcular la dirección hacia el objetivo
        Vector3 direction = (target.position - shootPoint.position).normalized;

        // Agregar velocidad al proyectil
        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = direction * projectileSpeed;
        }
    }
}
