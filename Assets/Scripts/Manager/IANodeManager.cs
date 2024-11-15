using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IANodeManager : MonoBehaviour
{
    [SerializeField] public Transform[] _nodes;
    public Transform[] Nodes => _nodes;

    private HashSet<Enemy> processedEnemies = new HashSet<Enemy>();

    private IEnumerator Start()
    {
        _nodes = GetComponentsInChildren<Transform>();
        yield return new WaitForSeconds(1f);
        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            enemy.NavMeshNodes.AddRange(_nodes);
            enemy.Initialize();
        }
    }
    /*
        private void Update()
        {
            foreach (Enemy enemy in GameManager.Instance.Enemies)
            {
                if (!processedEnemies.Contains(enemy))
                {
                    //enemy.NavMeshNodes.AddRange(_nodes);
                    processedEnemies.Add(enemy);
                }
            }
        }
    */
}
