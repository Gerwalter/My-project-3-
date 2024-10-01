using UnityEngine;

public class ImageChanger : MonoBehaviour
{
    public Texture2D[] textures;  // Array de texturas
    public float changeInterval = 0.5f;  // Intervalo entre cambios
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private int currentTextureIndex = 0;
    [SerializeField] private float timer = 0f;

    void Start()
    {
        // Obtén el componente Renderer del objeto 3D
        objectRenderer = GetComponent<Renderer>();

        // Asegúrate de que haya texturas en el array
        if (textures.Length > 0)
        {
            objectRenderer.material.mainTexture = textures[currentTextureIndex];
        }
    }

    void FixedUpdate()
    {
        // Incrementa el temporizador basado en FixedUpdate (normalmente usado para física)
        timer += Time.fixedDeltaTime;

        // Si ha pasado el intervalo, cambia a la siguiente textura
        if (timer >= changeInterval)
        {
            timer = 0f;  // Reinicia el temporizador

            // Cambia a la siguiente textura en el array
            currentTextureIndex = (currentTextureIndex + 1) % textures.Length;
            objectRenderer.material.mainTexture = textures[currentTextureIndex];
        }
    }
}
