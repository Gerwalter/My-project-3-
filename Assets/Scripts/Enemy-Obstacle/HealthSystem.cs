using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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

    public void ReceiveDamage(float damage)
    {
        GetLife -= damage;
        _bloodVFX.SendEvent("OnTakeDamage");
        OnTakeDamage?.Invoke();
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

    private bool isTakingContinuousDamage = false;
    public void ApplyContinuousDamageFromPlayer(float totalDamage, float duration)
    {
        if (isTakingContinuousDamage) return;

        isTakingContinuousDamage = true;
        StartCoroutine(ContinuousDamageRoutine(totalDamage, duration));
    }

    private IEnumerator ContinuousDamageRoutine(float totalDamage, float duration)
    {
        float damagePerTick = totalDamage / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ReceiveDamage(damagePerTick * Time.deltaTime);
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


    [SerializeField] private VisualEffect[] vfxArray;

    // Nombre del parámetro booleano en el VFX Graph, si es necesario
    [SerializeField] private string vfxParameter = "PlayVFX";

    public void PlayVFX()
    {
        foreach (var vfx in vfxArray)
        {
            if (vfx != null)
            {
                // Si el VFX Graph tiene un parámetro booleano para activar


                vfx.SetBool(vfxParameter, true);


                // Reiniciar el VFX para que las partículas comiencen de nuevo
                vfx.Reinit();
            }
            else
            {
                Debug.LogWarning("Un VisualEffect no está asignado en el array.");
            }
        }
    }
}
