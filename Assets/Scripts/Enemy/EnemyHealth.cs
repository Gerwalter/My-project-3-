using UnityEngine;

public class EnemyHealth : Health, IEnemy
{
    // Enemigo actualmente seleccionado/activo
    public static EnemyHealth enemyActivo;
    public Animator anim;
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
        }
    }
    virtual public void Start()
    {

        GetLife = maxHealth;
    }


    public override void OnTakeDamage(float damage)
    {
        GetLife -= damage;
        if (_bloodVFX != null)
            _bloodVFX.SendEvent("OnTakeDamage");
        anim.SetTrigger("Damage");
        if (GetLife <= 0)
            Die();
    }

    virtual public void Die()
    {
        Despawn();
    }

    public void Spawn(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
    }

    public void Despawn()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
