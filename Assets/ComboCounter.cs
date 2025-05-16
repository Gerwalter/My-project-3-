using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    public float comboResetTime = 3f;
    private float comboTimer;
    private bool comboActive = false;

    public int comboCount = 0;

    // Opcional: UI
    public TMP_Text comboText; // Arrástralo desde el Canvas
    private void Awake()
    {
        EventManager.Subscribe("RegisterHit", Hit);
    }
    private void Start()
    {
        comboText.text = "Combo x" + comboCount;
    }

    void Update()
    {
        if (comboActive)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer >= comboResetTime)
            {
                ResetCombo();
            }
        }
    }
    public void Hit(params object[] args)
    {
        RegisterHit();
    }
    public void RegisterHit()
    {
        comboCount++;
        comboTimer = 0f;
        comboActive = true;

        if (comboText != null)
            comboText.text = "Combo x" + comboCount;
    }

    public void ResetCombo()
    {
        if (comboCount > 0)
        {
            Debug.Log($"Combo perdido: {comboCount} golpes");
        }

        comboCount = 0;
        comboTimer = 0f;
        comboActive = false;

        if (comboText != null)
            comboText.text = "";
    }

    // Llama a esto desde tu sistema de daño cuando el jugador recibe un golpe
    public void OnPlayerDamaged()
    {
        ResetCombo();
    }
}
