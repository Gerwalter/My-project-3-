using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Rewind
{
    public int count;

    public override void Load()
    {
        if (!mementoState.IsRemember()) return;

        var remember = mementoState.Remember();

        gameObject.SetActive((bool)remember.parameters[0]);
    }

    public override void Save()
    {
        mementoState.Rec(gameObject.activeInHierarchy);
    }

    private void OnTriggerEnter(Collider other)
    {
        var p = other.GetComponent<Jogador>();

        if(p != null)
        {
            p.SetGold(count);
            gameObject.SetActive(false);
        }
    }
}
