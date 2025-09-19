using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertBar : MonoBehaviour, IAlertSystemObserver
{
    public Image imageBar;

    private void Awake()
    {
        // Accedemos al singleton
        ThiefAlertSystem system = ThiefAlertSystem.instance;

        if (system != null && system is IAlertSystemObservable observable)
        {
            observable.Subscribe(this);
        }
    }

    public void Notify(float Alert, float maxAlert)
    {
        float lifePercent = Alert / maxAlert;
        imageBar.fillAmount = lifePercent;
        imageBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }
}

