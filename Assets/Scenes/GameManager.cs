using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    [SerializeField] private List<Enemy> _enemies = new();
    public List<Enemy> Enemies
    {
        get { return _enemies; }
        set { _enemies = value; }
    }

    [SerializeField] private NavMeshSurface _surface;
    public NavMeshSurface Surface
    {
        get { return _surface; }
        set { _surface = value; }
    }

    [SerializeField] private IANodeManager _nodeManager;
    [SerializeField] private List<Transform> _nodes;

    public List<Transform> Nodes
    {
        get
        {
            if (NodeManager != null)
            {
                return new List<Transform>(NodeManager._nodes); // Devolvemos una copia
            }
            return new List<Transform>(); // Retornamos una lista vacía si no hay NodeManager
        }
    }

    public IANodeManager NodeManager
    {
        get { return _nodeManager; }
        set { _nodeManager = value; }
    }


}
