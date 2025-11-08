using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public float comboResetTime = 3f;
    private float comboTimer;
    private bool comboActive = false;

    public int comboCount = 0;

    [Header("UI Settings")]
    public Sprite[] numberSprites; // Sprites 0-9
    public Transform comboContainer; // Contenedor en el Canvas (con HorizontalLayoutGroup)
    public Image digitTemplate; // Imagen base (desactivada en el inspector)

    private void Awake()
    {
        EventManager.Subscribe("RegisterHit", Hit);
        EventManager.Subscribe("Damaged", OnPlayerDamaged);
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
        UpdateComboDisplay();
    }

    public void ResetCombo()
    {
        if (comboCount > 0)
            Debug.Log($"Combo perdido: {comboCount} golpes");

        comboCount = 0;
        comboTimer = 0f;
        comboActive = false;
        ClearComboDisplay();
    }

    private void UpdateComboDisplay()
    {
        if (comboContainer == null || numberSprites.Length == 0) return;

        ClearComboDisplay();

        string comboString = comboCount.ToString();

        foreach (char c in comboString)
        {
            int digit = c - '0';
            if (digit < 0 || digit > 9) continue;

            Image newDigit = Instantiate(digitTemplate, comboContainer);
            newDigit.sprite = numberSprites[digit];
            newDigit.gameObject.SetActive(true);
        }
    }

    private void ClearComboDisplay()
    {
        foreach (Transform child in comboContainer)
        {
            if (child != digitTemplate.transform)
                Destroy(child.gameObject);
        }
    }

    public void OnPlayerDamaged(params object[] args)
    {
        ResetCombo();
    }
}
