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
            return;
        }
    }
    #endregion

    [SerializeField] private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    [SerializeField] private PlayerStats _stats;
    public PlayerStats Stats
    {
        get { return _stats; }
        set { _stats = value; }
    }


    [SerializeField] private NavMeshSurface _surface;
    public NavMeshSurface Surface
    {
        get { return _surface; }
        set { _surface = value; }
    }


    [SerializeField] private IANodeManager _nodes;
    public IANodeManager Nodes
    {
        get { return _nodes; }
        set { _nodes = value; }
    }
}
