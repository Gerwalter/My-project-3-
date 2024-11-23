using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using static Player;

public class HealthSystem : MonoBehaviour, IDamaga
{
    [SerializeField] private float maxLife = 100;
    [SerializeField] private float currentLife;

    [Header("UI Settings")]
    [SerializeField] private Image healthBar;

    [SerializeField] private VisualEffect _bloodVFX;

    public event System.Action OnTakeDamage;
    public event System.Action OnHeal;
    public event System.Action OnDie;

    public float GetLife
    {
        get => currentLife;
        set
        {
            currentLife = Mathf.Clamp(value, 0, maxLife);
            UpdateHealthBar();
            if (currentLife == 0)
                 OnDie?.Invoke();
        }
    }

    private void Start()
    {

        currentLife = maxLife;
        UpdateHealthBar();

        // Suscribirse al evento OnDie para destruir el objeto
        OnDie += DestroyOnDeath;
    }

    public void Heal(float amount)
    {
        GetLife += amount;
        OnHeal?.Invoke();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }
    [SerializeField] private ElementType weakness; // Tipo de debilidad del enemigo
    [SerializeField] private float elementalMultiplier = 2.0f;
    public void ReceiveDamage(float damage, ElementType damageType)
    {
        if (damageType == weakness)
        {
            damage *= elementalMultiplier; // Aumenta el daño si coincide con la debilidad
        }

        GetLife -= damage;
        _bloodVFX.SendEvent("OnTakeDamage");
        OnTakeDamage?.Invoke();
    }

    private bool isTakingContinuousDamage = false;
    public void ApplyContinuousDamageFromPlayer(float totalDamage, float duration, ElementType damageType)
    {
        if (isTakingContinuousDamage) return;

        isTakingContinuousDamage = true;
        StartCoroutine(ContinuousDamageRoutine(totalDamage, duration, damageType));
    }

    private IEnumerator ContinuousDamageRoutine(float totalDamage, float duration, ElementType damageType)
    {
        float damagePerTick = totalDamage / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Verifica si el tipo de daño coincide con la debilidad
            float actualDamage = (damageType == weakness) ? damagePerTick * 2 : damagePerTick;


            int roundedDamage = Mathf.RoundToInt(actualDamage * Time.deltaTime);
            // Aplica el daño calculado
            ReceiveDamage(roundedDamage, damageType);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isTakingContinuousDamage = false;
    }


    private void DestroyOnDeath()
    {
        // Mensaje opcional para depuración
        Debug.Log($"{gameObject.name} has died and will be destroyed.");
        Destroy(gameObject);
    }
}
