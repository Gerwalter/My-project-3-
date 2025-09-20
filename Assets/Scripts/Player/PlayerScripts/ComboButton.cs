using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboButton : MonoBehaviour
{
    public string comboID;
    public float comboPrice;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BuyCombo);
    }

    private void BuyCombo()
    {
        if (ComboStore.Instance.Points >= comboPrice)
        {
            ComboStore.Instance.Points -= comboPrice;
            ComboUnlockManager.Instance.UnlockCombo(comboID);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No tienes suficientes puntos para el combo " + comboID);
        }
    }
}
