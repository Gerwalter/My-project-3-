using UnityEngine;

public class Entity : HP
{
  
    protected virtual void Awake()
    {
        SubscribeToHpEvents();
    }

   
    private void SubscribeToHpEvents()
    {
        OnDamageReceived += HandleDamage;
        OnDeath += HandleDeath;
        OnHeal += HandleHeal;
    }

    protected virtual void HandleDamage(float damage)
    {
        // Lógica adicional al recibir daño para los enemigos
        Debug.Log("Entity recibió daño: " + damage);
    }

    protected virtual void HandleDeath()
    {
        // Lógica de muerte específica para enemigos
        Debug.Log("Entity ha muerto");
        Destroy(gameObject, 2.0f); // Se destruye después de la animación de muerte
    }

    protected virtual void HandleHeal(float amount)
    {
        // Lógica adicional al curarse
    }
}
