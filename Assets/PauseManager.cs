using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Referencia al men� de pausa en la escena
    public bool isPaused = false; // Estado del juego (pausado o no)

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Detectar el input para pausar/reanudar (por defecto, la tecla Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // M�todo para pausar el juego
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Mostrar el men� de pausa
                                     // Time.timeScale = 0f; // Detener el tiempo del juego
        isPaused = true;

    }


    public void MenuDeactivate()
    {
        pauseMenuUI.SetActive(false);
    }

    // M�todo para reanudar el juego
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Ocultar el men� de pausa
       // Time.timeScale = 1f; // Restaurar el tiempo del juego
        isPaused = false;

    }
}