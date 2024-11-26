using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoldManager : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;


    public static GoldManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Busca el componente PlayerStats en el jugador
        _playerStats = GameManager.Instance.Stats;

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
