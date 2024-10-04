using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour, IInteract
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private VideoShader video;

    public void OnInteract()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Crea un nuevo color con los valores generados.
        Color randomColor = new Color(r, g, b);

        // Aplica el color al material del objeto.
        _renderer.material.color = randomColor;

        video.VideoPlay();

    }
}
