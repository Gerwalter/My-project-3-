using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    // Método llamado cuando otro objeto entra en un trigger colisionador
    public int scene;
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entró en el trigger tiene un tag específico (opcional)
        if (other.CompareTag("Player"))  // Puedes cambiar "Player" por cualquier otro tag
        {
            // Cargar la escena número 1
            SceneManager.LoadScene(scene);
        }
    }
}
