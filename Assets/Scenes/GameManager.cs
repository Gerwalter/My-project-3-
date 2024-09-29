using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Cam _camara;
    public Cam Camera
    {
        get { return _camara; }
        set { _camara = value; }
    }

[SerializeField]    private Player _player;
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
    public static void LoadLevel(string newSceneName)
    {
        SceneManager.LoadScene(newSceneName);
    }
}
