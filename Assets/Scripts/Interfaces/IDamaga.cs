using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamaga
{
    float GetLife { get; set; }

    void ReceiveDamage(float damage);
    void Heal(float amount);
}
