using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AlertThreshold
{
    [Range(0f, 1f)] public float percent;   // Nivel de alerta (0 a 1)
    public string triggerName;              // Trigger en el Animator
}

public class AlertBar : MonoBehaviour, IAlertSystemObserver
{
    [Header("Componentes")]
    public Animator alertAnimator;
    public ThiefAlertSystem system;

    [Header("Configuración de niveles de alerta")]
    public List<AlertThreshold> thresholds = new List<AlertThreshold>()
    {
        new AlertThreshold() { percent = 0.2f, triggerName = "AlertLevel1" },
        new AlertThreshold() { percent = 0.4f, triggerName = "AlertLevel2" },
        new AlertThreshold() { percent = 0.6f, triggerName = "AlertLevel3" },
        new AlertThreshold() { percent = 0.8f, triggerName = "AlertLevel4" },
        new AlertThreshold() { percent = 1.0f, triggerName = "AlertMax" }
    };

    private HashSet<string> triggeredLevels = new HashSet<string>();

    private void Start()
    {
        system = ThiefAlertSystem.instance;

        if (system != null && system is IAlertSystemObservable observable)
        {
            observable.Subscribe(this);
        }
    }

    public void Notify(float Alert, float maxAlert)
    {
        float percent = Alert / maxAlert;

        foreach (var threshold in thresholds)
        {
            // Si se supera un nivel que aún no se activó
            if (percent >= threshold.percent && !triggeredLevels.Contains(threshold.triggerName))
            {
                if (alertAnimator != null && !string.IsNullOrEmpty(threshold.triggerName))
                {
                    alertAnimator.SetTrigger(threshold.triggerName);
                    triggeredLevels.Add(threshold.triggerName);
                }
            }
            // Si baja por debajo del umbral, permitir reactivarlo
            else if (percent < threshold.percent && triggeredLevels.Contains(threshold.triggerName))
            {
                triggeredLevels.Remove(threshold.triggerName);
            }
        }
    }
}
