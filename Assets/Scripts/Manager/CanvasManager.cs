using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public void LoadSceneAsync(string scene)
    {
        SceneLoaderManager.Instance.LoadSecenAsync(scene);
    }

    // M�todo para cambiar canvas usando un int

    public void CloseUp()
    {
        Application.Quit();
    }
}
