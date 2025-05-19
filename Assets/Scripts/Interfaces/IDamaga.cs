using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public interface IDamaga
{
    float GetLife { get; set; }

    void Heal(float amount);
}
