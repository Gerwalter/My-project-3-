using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILifeObserver
{
    void Notify(float life, float maxLife);
}
