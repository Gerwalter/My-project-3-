using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Collider _col;
    public MeshRenderer _renderer;
    public NavMeshModifier _mod;


    public void Activated()
    {
        _col.enabled = true;
        _mod.enabled = true;
        _renderer.enabled = true;
        GameManager.Instance.Surface.BuildNavMesh();
    }

    public void Deactivated()
    {
        _col.enabled = false;
        _mod.enabled = false;
        _renderer.enabled = false;
        GameManager.Instance.Surface.BuildNavMesh();
    }
}
