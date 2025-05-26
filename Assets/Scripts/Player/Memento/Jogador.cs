using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jogador : Rewind
{
    public float life;
    public int gold;
    public float speed;
    public GameManagerMemento memento;
    private void Update()
    {


        if(Input.GetKeyDown(KeyCode.M))
        {
            var x = memento;
            x.Saver();
        }
    }

    public void SetGold(int addGold)
    {
        gold += addGold;
    }

    public override void Save()
    {
        mementoState.Rec(life, gold, transform.position);
    }

    public override void Load()
    {
        if (!mementoState.IsRemember()) return;

        var remember = mementoState.Remember();

        life = (float)remember.parameters[0];
        gold = (int)remember.parameters[1];
        transform.position = (Vector3)remember.parameters[2];
    }
}
