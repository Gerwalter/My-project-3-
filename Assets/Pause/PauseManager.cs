using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Asigna el menú de pausa en el Inspector
    public GameObject OptionsMenuUI; // Asigna el menú de pausa en el Inspector
    public bool isPaused = false;
    public bool isOptions = false;
    private bool canPause = true; // Variable para controlar si se puede pausar

    private void Start()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(false);
        isOptions = false;
        // Verifica si estás en la escena 0
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            canPause = false; // No se puede pausar en la escena 0
            return; // Sale de la función Start
        }

        isPaused = false;
        pauseMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(false);
        isOptions = false;
    }

    void Update()
    {
        if (canPause && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!canPause) return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenuUI.SetActive(isPaused);
    }
    public void ToggleOptions()
    {
        isOptions = !isOptions;
        OptionsMenuUI.SetActive(isOptions);
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1; // Asegura que el tiempo se reanude al salir
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu"); // Cambia "MainMenu" por el nombre de tu escena principal
    }
}
