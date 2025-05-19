using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBat : MonoBehaviour, IObserver
{
    public GameObject observable;
    public Image imageBar;
    private void Awake()
    {
        if (observable.GetComponent<IObservable>() != null)
            observable.GetComponent<IObservable>().Subscribe(this);
    }
    public void Notify(float life, float maxLife)
    {
        float fillAmount = life / maxLife;
        imageBar.fillAmount = fillAmount;

        // Lerp de verde (stamina llena) a rojo (stamina vacía)
        imageBar.color = Color.Lerp(Color.red, Color.green, fillAmount);
        imageBar.gameObject.SetActive(fillAmount < 1.0f);
    }
}
