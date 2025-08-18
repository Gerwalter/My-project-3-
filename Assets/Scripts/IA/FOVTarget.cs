using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTarget : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshR;

    protected virtual void Start()
    {
        if (_meshR == null) _meshR = GetComponent<MeshRenderer>();
    }

    public void ChangeColor(Color color) => _meshR.material.color = color;
}
