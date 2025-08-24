using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertBar : MonoBehaviour, IAlertSystemObserver
{
    public void Notify(float Alert, float maxAlert)
    {
        float lifePercent = Alert / maxAlert;
        imageBar.fillAmount = lifePercent;
        imageBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    public GameObject observable;
    public Image imageBar;
    private void Awake()
    {
        if (observable.GetComponent<IAlertSystemObservable>() != null)
            observable.GetComponent<IAlertSystemObservable>().Subscribe(this);
        imageBar.color = Color.green;
    }
}
