using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlertSystemObservable
{
    void Subscribe(IAlertSystemObserver x);
    void Unsubscribe(IAlertSystemObserver x);
}
