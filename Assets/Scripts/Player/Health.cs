using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    public VisualEffect _bloodVFX; // Asigna el sistema de part�culas desde el inspector
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

    // Este m�todo se puede llamar cuando el objeto recibe da�o
    public virtual void OnTakeDamage(float damageAmount)
    {       

    }
}
