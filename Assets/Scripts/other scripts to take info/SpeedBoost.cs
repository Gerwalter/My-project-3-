using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private Player _player; // Multiplicador de velocidad
    private void Start()
    {
        _player = GameManager.Instance.Player;
    }
    private void OnTriggerEnter(Collider other)
    {
        _player.SpeedBooster();
        Destroy(gameObject); // Destruye el objeto del boost
    }
}
