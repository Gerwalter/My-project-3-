using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger: MonoBehaviour
{
    public string[] Scenes; // Nombres de las escenas a cargar
    [SerializeField] private int currentSceneIndex = -1; // Índice de la escena actual (-1 significa ninguna)
    [SerializeField] private SceneLoaderManager loadingScreen;

    void Start()
    {
        loadingScreen = SceneLoaderManager.Instance;
        LoadNextScene();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
            LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (currentSceneIndex >= 0 && currentSceneIndex < Scenes.Length)
        {
            SceneManager.UnloadSceneAsync(Scenes[currentSceneIndex]);
        }

        // Incrementamos el índice para cargar la siguiente escena
        currentSceneIndex++;

        // Verificamos si el índice está dentro del rango del array
        if (currentSceneIndex < Scenes.Length)
        {
            SceneManager.LoadScene(Scenes[currentSceneIndex], LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("No hay más escenas para cargar.");
        }
    }
}

