using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private Vector3 savedPlayerPosition;
    private string savedSceneName;
    private bool returningFromBattle = false;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartBattle(Vector3 playerPosition, string currentScene, string battleScene)
    {
        savedPlayerPosition = playerPosition;
        savedSceneName = currentScene;
        returningFromBattle = false;

        SceneManager.LoadScene(battleScene);
    }

    public void EndBattle()
    {
        returningFromBattle = true;
        SceneManager.LoadScene(savedSceneName);
    }

    // La firma correcta para el evento
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (returningFromBattle && scene.name == savedSceneName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = savedPlayerPosition;
            }

            returningFromBattle = false;
        }
    }
}
