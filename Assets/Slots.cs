using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : ButtonBehaviour
{
    [SerializeField] private Renderer _renderer;

    public override void OnInteract()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Crea un nuevo color con los valores generados.
        Color randomColor = new Color(r, g, b);
    }
}
