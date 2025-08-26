using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : Coin
{
    public int GoldAmmount;
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Si el objeto con el que chocó tiene el GoldManager
        if (other.TryGetComponent<GoldManager>(out GoldManager intObj))
        {
            Colect();
        }
    }
    public override void Colect()
    {
        EventManager.Trigger("IncreaseGold", GoldAmmount);
        collected = true;
        Destroy(gameObject); // Destruye la moneda
    }
}
