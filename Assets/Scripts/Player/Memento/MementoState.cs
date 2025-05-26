using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoState 
{
    List<ParamsMemento> _parameters = new List<ParamsMemento>();
    public void Rec(params object[] parameters)
    {
        if (_parameters.Count > 500)
            _parameters.RemoveAt(0);

        var remember = new ParamsMemento(parameters);
        _parameters.Add(remember);
    }

    public bool IsRemember()
    {
        return _parameters.Count > 0;
    }

    public ParamsMemento Remember()
    {
        var x = _parameters[_parameters.Count - 1];
        _parameters.RemoveAt(_parameters.Count - 1);

        return x;
    }
}
