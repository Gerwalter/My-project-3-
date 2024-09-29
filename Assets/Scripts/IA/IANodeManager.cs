using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IANodeManager : MonoBehaviour
{
    [SerializeField] private Transform[] _nodes;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        _nodes = GetComponentsInChildren<Transform>();

        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            enemy.NavMeshNodes.AddRange(_nodes);
            enemy.Initialize();
        }
    }


    public void NodesExtraConfirm()
    {
        print("funca");
        _nodes = GetComponentsInChildren<Transform>();

        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            enemy.NavMeshNodes.AddRange(_nodes);
        }
    }
}
