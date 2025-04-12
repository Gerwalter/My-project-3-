using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILifeObservable
{
    void Subscribe(ILifeObserver x);
    void Unsubscribe(ILifeObserver x);
}