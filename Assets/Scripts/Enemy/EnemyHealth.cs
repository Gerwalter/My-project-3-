using System.Collections;
using UnityEngine;

public class EnemyHealth : Health, IEnemy
{
    // Enemigo actualmente seleccionado/activo
    public static EnemyHealth enemyActivo;
    public Animator anim;
    public Material Mat;
    private Coroutine damageFlashRoutine;
    public float flashDuration = 0.2f; // Velocidad del flash
    private void OnEnable()
    {
        EventManager.Subscribe("SendDamage", TakeDamage);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe("SendDamage", TakeDamage);
    }

    private void TakeDamage(params object[] parametros)
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

        // Inicia el flash del material
        if (damageFlashRoutine != null)
            StopCoroutine(damageFlashRoutine);

        damageFlashRoutine = StartCoroutine(DamageFlash());

        if (GetLife <= 0)
            anim.SetTrigger("Death");
    }
    private IEnumerator DamageFlash()
    {
        if (Mat == null) yield break;

        Color startColor = Color.white;
        Color hitColor = Color.red;

        float t = 0f;

        // Blanco → Rojo
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            Mat.SetColor("_DiffuseColor", Color.Lerp(startColor, hitColor, t));
            yield return null;
        }

        t = 0f;

        // Rojo → Blanco
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            Mat.SetColor("_DiffuseColor", Color.Lerp(hitColor, startColor, t));
            yield return null;
        }

        Mat.SetColor("_DiffuseColor", startColor);
    }

    virtual public void Die()
    {
        Despawn();
    }

    public AttackSlotType attackSlotType;
    public void Spawn(Vector3 position, AttackSlotType slotType)
    {
        transform.position = position;
        attackSlotType = slotType;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
