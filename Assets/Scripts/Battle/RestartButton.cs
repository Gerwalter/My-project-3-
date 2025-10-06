using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public string loadingSceneName = "LoadingScene"; // Nombre de la escena de carga

    // Llamar a este método desde el botón
    public void RestartLevel()
    {
        // Obtener la escena actual
        string currentScene = SceneManager.GetActiveScene().name;

        // Configurar el nombre de la próxima escena
        LoadingScreen.nextScene = currentScene;

        // Cargar la escena de pantalla de carga
        SceneManager.LoadScene(loadingSceneName);
    }
}
