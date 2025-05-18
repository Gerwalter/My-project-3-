using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class HealthTest : HP
{
    public override void ReciveDamage(float damage)
    {
        GetLife =- damage;
    }
}
