using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    private PlayerStats _playerStats;

    void Start()
    {
        // Busca el componente PlayerStats en el jugador
        _playerStats = FindObjectOfType<PlayerStats>();

        if (_playerStats == null)
        {
            Debug.LogError("No se encontró PlayerStats en la escena.");
        }
    }
    public int GetGold()
    {
        if (_playerStats == null) return 0;

        return _playerStats.gold;
    }

    public bool SpendGold(int amount)
    {
        if (_playerStats == null) return false;

        if (_playerStats.gold >= amount)
        {
            _playerStats.gold -= amount;
            return true;
        }

        return false;
    }

    public bool AddGold(int amount)
    {
        if (_playerStats == null) return false;

        if (_playerStats.gold >= amount)
        {
            _playerStats.gold += amount;
            return true;
        }

        return false;
    }
}
