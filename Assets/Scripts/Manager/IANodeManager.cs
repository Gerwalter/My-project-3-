using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IANodeManager : MonoBehaviour
{
    [SerializeField] public Transform[] _nodes;

    private void Awake()
    {
        GameManager.Instance.Nodes = this;
    }

    private void Start()
    {
        _nodes = GetComponentsInChildren<Transform>();
        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            enemy.NavMeshNodes.AddRange(_nodes);
            enemy.Initialize();
        }
    }
}
