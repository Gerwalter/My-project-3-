using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeButton : ButtonBehaviour
{

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Bridge _bridge;


    public override void OnInteract()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Crea un nuevo color con los valores generados.
        Color randomColor = new Color(r, g, b);

        // Aplica el color al material del objeto.
        _renderer.material.color = randomColor;

        if (_bridge != null)
        {
            _bridge.Activated();
        }

    }
}
