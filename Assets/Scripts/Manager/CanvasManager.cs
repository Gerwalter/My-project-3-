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


    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseUp()
    {
        Application.Quit();
    }
}
