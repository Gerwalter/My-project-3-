using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : HP
{
    public override void ReciveDamage(float damage)
    {
        EventManager.Trigger("RegisterHit", 4);
        GetLife =- damage;
    }
}
