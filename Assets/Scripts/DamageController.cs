using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{

    [SerializeField] private HP _playerHP;

    private void Start()
    {
        _playerHP = GameManager.Instance.Player.GetComponent<HP>();
        FullScreenManager.Instance.VignettePostProcess.SetFloat(FullScreenManager.Instance.VignetteAmountName, 5.0f);
    }

    private void Update()
    {
        float currentLife = _playerHP.GetLife;
        float maxLife = _playerHP.maxLife;

        // Calcular la cantidad de viñeta inversa: máximo con vida máxima, mínimo con vida baja
        float vignetteAmount = Mathf.Lerp(5.0f, 0.0f, 1 - (currentLife / maxLife));

        FullScreenManager.Instance.VignettePostProcess.SetFloat(FullScreenManager.Instance.VignetteAmountName, vignetteAmount);
    }
}
