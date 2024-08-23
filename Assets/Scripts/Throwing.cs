using UnityEngine;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public GameObject ObjectToThrow;

    [Header("Settings")]
    public int totalThorws;
    public int maxThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
        maxThrows = totalThorws;
    }

    private void Update()
    {
        if (Input.GetKey(throwKey) && readyToThrow && totalThorws > 0)
        {
            Throw();
        }
        if (Input.GetKeyDown(reloadKey) && totalThorws == 0)
        {
            Reload();
        }
    }

    void Throw()
    {
        readyToThrow = false;

        GameObject projectile = Instantiate(ObjectToThrow, attackPoint.position, attackPoint.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = attackPoint.forward;

        Vector3 forceToAdd = forceDirection * throwForce + attackPoint.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThorws--;

        Invoke(nameof(ResetThrows), throwCooldown);
        Destroy(projectile, 5f);
    }

    public void MaxThrowsReset()
    {
        maxThrows = totalThorws;
    }

    private void Reload()
    {
        totalThorws = maxThrows;
    }

    private void ResetThrows()
    {
        readyToThrow = true;
    }
}
