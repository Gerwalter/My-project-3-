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

    // Cambiar a una escena espec�fica por nombre
    public void LoadLevelByName(string levelName)
    {
        // Cargar una escena espec�fica por nombre
        SceneManager.LoadScene(levelName);
    }

    // Cambiar a una escena espec�fica por �ndice
    public void LoadLevelByIndex(int index)
    {
        // Cargar una escena espec�fica por �ndice
        SceneManager.LoadScene(index);
    }

    // Cargar el siguiente nivel en el �ndice de las escenas
    public void LoadNextLevel()
    {
        // Obtener el �ndice de la escena actual
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
        // En la versi�n final del juego, cerrar la aplicaci�n
        Application.Quit();
#endif
    }
}
