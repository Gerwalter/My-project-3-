using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFactory : Factory<Coin>
{
    [SerializeField] private Coin[] _coins;
    private Dictionary<string, Coin> _coinDic = new Dictionary<string, Coin>();

    private void Awake()
    {
        foreach (var coin in _coins)
        {
            if (!_coinDic.ContainsKey(coin.CoinID))
            {
                _coinDic.Add(coin.CoinID, coin);
            }
            else
            {
                Debug.LogWarning("CoinFactory: Duplicado de coinID -> " + coin.CoinID);
            }
        }
    }
    public string GetRandomCoinID()
    {
        if (_coins.Length == 0) return null;
        int randomIndex = Random.Range(0, _coins.Length);
        return _coins[randomIndex].CoinID;
    }
    public override Coin Create(string id)
    {
        if (_coinDic.ContainsKey(id))
        {
            var newCoin = Instantiate(_coinDic[id]);
            return newCoin;
        }

        Debug.LogError("CoinFactory: No se encontró moneda con ID -> " + id);
        return null;
    }
}