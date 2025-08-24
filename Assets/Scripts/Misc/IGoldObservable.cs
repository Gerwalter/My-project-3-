using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoldObservable
{    void Subscribe(IGoldObserver x);
    void Unsubscribe(IGoldObserver x);
}
