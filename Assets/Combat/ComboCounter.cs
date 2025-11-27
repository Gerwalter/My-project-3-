using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public float comboResetTime = 3f;
    private float comboTimer;
    private bool comboActive = false;

    public int comboCount = 0;

    [Header("UI (Sprites)")]
    public Image comboPrefix; // Sprite para el "Combo" o "x"
    public Transform digitsParent; 
    public Sprite[] numberSprites; 
    public GameObject digitPrefab; 

    private List<Image> activeDigits = new List<Image>();

    private void Awake()
    {
        EventManager.Subscribe("RegisterHit", Hit);
        EventManager.Subscribe("Damaged", OnPlayerDamaged);
        SceneManager.activeSceneChanged += OnSceneChanged;
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

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        EventManager.Unsubscribe("RegisterHit", Hit);
        EventManager.Unsubscribe("Damaged", OnPlayerDamaged);

        EventManager.Subscribe("RegisterHit", Hit);
        EventManager.Subscribe("Damaged", OnPlayerDamaged);
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
