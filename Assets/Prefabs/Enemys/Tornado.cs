using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : BossAttacks
{
    public override void ExecuteAttack(Transform target)
    {
        Debug.Log("Casting a Tornado at " + target.name +  "Damage");
        // L�gica espec�fica del ataque Fireball
    }
}
