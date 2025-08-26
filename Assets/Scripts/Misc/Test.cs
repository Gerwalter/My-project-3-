using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Factory<Coin> factory;
    Coin _coin;
    void Start()
    {
        _coin = factory.Create("GoldCoin");
    }
}
