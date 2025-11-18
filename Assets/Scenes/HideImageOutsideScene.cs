using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HideImageOutsideScene : MonoBehaviour
{

    [Header("Nombre de la escena donde la imagen debe verse")]
    public string targetSceneName = "NombreDeLaEscena";

    [Header("Imágenes a controlar")]
    public Image[] images;

    void Start()
    {
        UpdateImageVisibility();

        // Por si cambias de escena mientras el objeto persiste
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateImageVisibility();
    }

    void UpdateImageVisibility()
    {
        bool isTargetScene = SceneManager.GetActiveScene().name == targetSceneName;
        float alpha = isTargetScene ? 1f : 0f;

        foreach (var img in images)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }
}
