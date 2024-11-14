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
        // L�gica adicional al recibir da�o para los enemigos
        Debug.Log("Entity recibi� da�o: " + damage);
    }

    protected virtual void HandleDeath()
    {
        // L�gica de muerte espec�fica para enemigos
        Debug.Log("Entity ha muerto");
        Destroy(gameObject, 2.0f); // Se destruye despu�s de la animaci�n de muerte
    }

    protected virtual void HandleHeal(float amount)
    {
        // L�gica adicional al curarse
    }
}
