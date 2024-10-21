using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    // M�todo llamado cuando otro objeto entra en un trigger colisionador
    public int scene;
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entr� en el trigger tiene un tag espec�fico (opcional)
        if (other.CompareTag("Player"))  // Puedes cambiar "Player" por cualquier otro tag
        {
            // Cargar la escena n�mero 1
            SceneManager.LoadScene(scene);
        }
    }
}
