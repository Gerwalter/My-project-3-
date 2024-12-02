using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasMenuManager : MonoBehaviour
{
    // Referencia al panel que quieres activar/desactivar
    public GameObject panel;

    public static CanvasMenuManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        panel.SetActive(false);
    }



    void Update()
    {
        // Verificar si se presiona la tecla Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Verificar si no estamos en la escena 0
            if (SceneManager.GetActiveScene().buildIndex != 1)
            {
                // Cambiar el estado activo del panel
                panel.SetActive(!panel.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                // Cambiar el estado activo del panel
                panel.SetActive(!panel.activeSelf);
            }
        }
    }
}
