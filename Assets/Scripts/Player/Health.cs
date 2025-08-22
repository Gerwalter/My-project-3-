using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    public VisualEffect _bloodVFX; // Asigna el sistema de partículas desde el inspector
    public float maxHealth = 100;
    private float currentHealth;
    public float GetLife
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Este método se puede llamar cuando el objeto recibe daño
    public virtual void OnTakeDamage(float damageAmount)
    {       

    }
}
