using UnityEngine;

public class ThrowChange : MonoBehaviour
{
    public Throwing thro;
    public int newTotal = 0;
    public float newcooldown = 0;
    public float newforce = 0;

    private void OnTriggerEnter(Collider other)
    {
        thro.totalThorws = newTotal;
        thro.MaxThrowsReset();
        thro.throwCooldown = newcooldown;
        thro.throwForce = newforce;
        Destroy(gameObject);
    }
}
