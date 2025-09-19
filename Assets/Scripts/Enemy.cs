using UnityEngine;

public class Enemy : Health
{
    // Enemigo actualmente seleccionado/activo
    public static Enemy enemyActivo;

    private void OnEnable()
    {
        EventManager.Subscribe("SendDamage", RecibirDanio);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe("SendDamage", RecibirDanio);
    }

    private void RecibirDanio(params object[] parametros)
    {
        float damage = (float)parametros[0];
        GameObject objetivo = (GameObject)parametros[1];

        // Solo aplicar si el mensaje viene dirigido a este enemigo
        if (objetivo == gameObject)
        {
            OnTakeDamage(damage);
            print(GetLife);
        }
    }
    void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
        GetLife = maxHealth;
    }


    public override void OnTakeDamage(float damage)
    {
        GetLife -= damage;
        if (_bloodVFX != null)
            _bloodVFX.SendEvent("OnTakeDamage");

        if (GetLife <= 0)
            Die();
    }

    private void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        Destroy(gameObject);
    }

}
