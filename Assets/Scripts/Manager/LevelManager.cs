using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public CameraController controller;
    public GameObject panel;
    // Reinicia la escena actual

    private void Start()
    {
        controller = FindObjectOfType<CameraController>();
    }
    public void RestartLevel()
    {
        // Obtener la escena actual y reiniciarla
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        controller.IsCameraFixed = false;
        panel.SetActive(false);

    }

    // Cambiar a una escena específica por nombre
    public void LoadLevelByName(string levelName)
    {
        // Cargar una escena específica por nombre
        SceneManager.LoadScene(levelName);
    }

    // Cambiar a una escena específica por índice
    public void LoadLevelByIndex(int index)
    {
        // Cargar una escena específica por índice
        SceneManager.LoadScene(index);
    }

    // Cargar el siguiente nivel en el índice de las escenas
    public void LoadNextLevel()
    {
        // Obtener el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Cargar la siguiente escena
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Salir del juego
    public void QuitGame()
    {
        // Si estamos en el editor de Unity, usar esto
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // En la versión final del juego, cerrar la aplicación
        Application.Quit();
#endif
    }
}
