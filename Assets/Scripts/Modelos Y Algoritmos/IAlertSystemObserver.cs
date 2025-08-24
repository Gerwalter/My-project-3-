using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlertSystemObserver
{
    void Notify(float Alert, float maxAlert);
}
