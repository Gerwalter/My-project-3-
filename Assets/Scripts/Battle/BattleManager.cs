using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private Vector3 savedPlayerPosition;
    private string savedSceneName;

    private void Awake()
    {
        // Asegurarse de que solo exista un BattleManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Llamar cuando empiece una batalla
    /// </summary>
    public void StartBattle(Vector3 playerPosition, string currentScene, string battleScene)
    {
        savedPlayerPosition = playerPosition;
        savedSceneName = currentScene;

        // Cargar escena de batalla
        SceneManager.LoadScene(battleScene);
    }

    /// <summary>
    /// Llamar cuando la batalla termine
    /// </summary>
    public void EndBattle()
    {
        SceneManager.LoadScene(savedSceneName);
        // Usamos SceneManager.sceneLoaded para reubicar al jugador al cargar
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == savedSceneName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = savedPlayerPosition;
            }

            // Dejar de escuchar el evento
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
