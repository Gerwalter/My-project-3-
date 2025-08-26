using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Coin : MonoBehaviour
{
    [SerializeField] private string coinID; // ID único para identificar el tipo de moneda

    public string CoinID => coinID;
    public virtual void Colect()
    {
        Debug.Log("Recolectada: " + coinID);
    }
}
