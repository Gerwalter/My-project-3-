using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Player
{


    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image _staminaBar;

    private void Start()
    {
        GetLife = maxLife;
        UpdateHealthBar();
    }
    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

}
