using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGBooster : MonoBehaviour
{
    [SerializeField] private PlayerAttack _player; // Multiplicador de velocidad
    private void Start()
    {
        //_player = GameManager.Instance.Player;
    }
    private void OnTriggerEnter(Collider other)
    {
        _player.DMGBooster();
        Destroy(gameObject); // Destruye el objeto del boost
    }
}
