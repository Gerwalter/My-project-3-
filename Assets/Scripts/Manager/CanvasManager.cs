using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public void LoadSceneAsync(string scene)
    {
        SceneLoaderManager.Instance.LoadSecenAsync(scene);
    }

    // Método para cambiar canvas usando un int

    public void CloseUp()
    {
        Application.Quit();
    }
}
