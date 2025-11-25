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

    [Header("Transforms cuyos hijos (Images) deben ocultarse también")]
    public Transform[] imageParents;

    void Start()
    {
        UpdateImageVisibility();
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

        // Ocultar imágenes directas
        foreach (var img in images)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }

        // Ocultar imágenes hijas (como los dígitos del ComboCounter)
        foreach (var parent in imageParents)
        {
            if (parent != null)
            {
                foreach (Transform child in parent)
                {
                    Image img = child.GetComponent<Image>();
                    if (img != null)
                    {
                        Color c = img.color;
                        c.a = alpha;
                        img.color = c;
                    }
                }
            }
        }
    }
}
