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

    [Header("UI (Sprites)")]
    public Image comboPrefix; // Sprite para el "Combo" o "x"
    public Transform digitsParent; // Contenedor donde se mostrarán los dígitos
    public Sprite[] numberSprites; // Array de sprites de 0-9
    public GameObject digitPrefab; // Prefab con un Image (para cada dígito)

    private List<Image> activeDigits = new List<Image>();

    private void Awake()
    {
        EventManager.Subscribe("RegisterHit", Hit);
        EventManager.Subscribe("Damaged", OnPlayerDamaged);
    }

    private void Start()
    {
        ClearDigits();
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
        {
            Debug.Log($"Combo perdido: {comboCount} golpes");
        }

        comboCount = 0;
        comboTimer = 0f;
        comboActive = false;

        ClearDigits();
    }

    private void UpdateComboDisplay()
    {
        ClearDigits();

        string comboString = comboCount.ToString();

        foreach (char c in comboString)
        {
            int digit = c - '0';
            GameObject newDigit = Instantiate(digitPrefab, digitsParent);
            Image img = newDigit.GetComponent<Image>();
            img.sprite = numberSprites[digit];
            activeDigits.Add(img);
        }
    }

    private void ClearDigits()
    {
        foreach (var img in activeDigits)
        {
            if (img != null)
                Destroy(img.gameObject);
        }
        activeDigits.Clear();
    }

    public void OnPlayerDamaged(params object[] args)
    {
        ResetCombo();
    }
}
