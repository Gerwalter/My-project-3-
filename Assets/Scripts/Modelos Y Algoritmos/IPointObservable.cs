using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPointObservable
{
    void Subscribe(IPointObserver x);
    void Unsubscribe(IPointObserver x);
}
