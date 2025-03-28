using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
public class ObjectPool <T>
{
   List<T> Stock = new List<T>();
    Action<T> On;
    Action<T> Off;
    Func<T> Factory;

    public ObjectPool(Func<T> _Factory, Action<T> ObjOn, Action<T> ObjOff,  int currentStock = 5)
    {
        On = ObjOn;
        Off = ObjOff;
        Factory = _Factory;
        for (int i = 0; i < currentStock; i++)
        {
             var x = Factory();
            Stock.Add(x);
        }
    }

    public T Get()
    {
        T x;
        if (Stock.Count > 0)
        {
            x = Stock[0];
            Stock.Remove(x);

        }
        else
        {
            x = Factory(); 
        }

        On(x);
        return x;

    }

    public void Return(T objx) 
        { }
}
