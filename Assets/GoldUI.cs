using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText; // Referencia al texto de la UI
    [SerializeField] private GoldManager _goldManager;

    void Start()
    {
        _goldManager = FindObjectOfType<GoldManager>();

        goldText = CanvasReferencesManager.Instance.GoldText;

        if (_goldManager == null)
        {
            Debug.LogError("No se encontró GoldManager en la escena.");
        }
    }

    void Update()
    {
        if (_goldManager != null)
        {
            goldText.text = $"Gold: {_goldManager.GetGold()}";
        }
    }
}
