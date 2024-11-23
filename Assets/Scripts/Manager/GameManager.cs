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

    [SerializeField] private Animator _playeranim;
    public Animator PlayerAnim
    {
        get { return _playeranim; }
        set { _playeranim = value; }
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


    [SerializeField] private IANodeManager _nodes;
    public IANodeManager Nodes
    {
        get { return _nodes; }
        set { _nodes = value; }
    }
}
