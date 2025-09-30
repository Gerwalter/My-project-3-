using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarLife : MonoBehaviour, ILifeObserver
{
    public GameObject observable;
    public Image imageBar;
    private void Awake()
    {
        if(observable.GetComponent<ILifeObservable>() != null)
            observable.GetComponent<ILifeObservable>().Subscribe(this);
        imageBar.color = Color.green;
    }
    public void Notify(float life, float maxLife)
    {
        float lifePercent = life / maxLife;
        imageBar.fillAmount = lifePercent;
       // imageBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }
}
