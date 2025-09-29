using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private Vector3 savedPlayerPosition;
    private string savedSceneName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle(Vector3 playerPosition, string currentScene, string battleScene)
    {
        savedPlayerPosition = playerPosition;
        savedSceneName = currentScene;

        // Usamos la pantalla de carga
        LoadingScreen.nextScene = battleScene;
        SceneManager.LoadScene("LoadingScene");
    }

    public void EndBattle()
    {
        LoadingScreen.nextScene = savedSceneName;
        SceneManager.LoadScene("LoadingScene");

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

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
