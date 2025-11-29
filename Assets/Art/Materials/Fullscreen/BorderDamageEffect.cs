using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderDamageEffect : MonoBehaviour, ILifeObserver
{
    [Header("Asignar Material con el parámetro _ScreenBorderAmount")]
    [SerializeField] private Material screenBorderMaterial;

    [Header("Límites del efecto")]
    [SerializeField] private float maxBorderValue = 5f; // Vida completa
    [SerializeField] private float minBorderValue = 1f; // Vida mínima

    [Header("Referencia al PlayerUI")]
    [SerializeField] private PlayerUI playerUI;

    private void Start()
    {
        if (playerUI != null)
            playerUI.Subscribe(this);
    }

    private void OnDestroy()
    {
        if (playerUI != null)
            playerUI.Unsubscribe(this);
    }

    public void Notify(float currentLife, float maxLife)
    {
        float lifePercent = currentLife / maxLife; // 1 = vida completa, 0 = muerto

        // Mapea el porcentaje al rango 5 1
        float newValue = Mathf.Lerp(minBorderValue, maxBorderValue, lifePercent);

        // Aplica al material
        if (screenBorderMaterial != null)
        {
            screenBorderMaterial.SetFloat("_ScreenBorderAmount", newValue);
        }
    }
}
