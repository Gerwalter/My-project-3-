using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("main menu")]
    [SerializeField] private Canvas[] _mainMenuScreens;

    private Canvas _activeScene;
    public GameObject[] Scene;

    public enum Menu
    {
        MainMenu,
        Options,
        Credits,
        // Añade más menús según sea necesario
    }

    private void Start()
    {
        if (_mainMenuScreens.Length > 0)
        {
            for (int i = 0; i < _mainMenuScreens.Length; i++)
            {
                if (i != 0)
                {
                    _mainMenuScreens[i].enabled = false;
                }
                else
                {
                    _activeScene = _mainMenuScreens[i];
                }
            }
        }
    }

    // Método para cambiar canvas usando el enum
    public void ChangeCanvas(Menu menu)
    {
        _activeScene.enabled = false;
        _activeScene = _mainMenuScreens[(int)menu];
        _activeScene.enabled = true;
    }

    // Método para cambiar canvas usando un int
    public void ChangeCanvas(int index)
    {
        if (index < 0 || index >= _mainMenuScreens.Length)
        {
            Debug.LogError("Índice de menú fuera de rango");
            return;
        }
        _activeScene.enabled = false;
        _activeScene = _mainMenuScreens[index];
        _activeScene.enabled = true;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseUp()
    {
        Application.Quit();
    }
}
