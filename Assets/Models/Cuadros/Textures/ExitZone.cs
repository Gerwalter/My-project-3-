using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    [Tooltip("Nombre de la escena de victoria a cargar")]
    public string victorySceneName = "VictoryScene";

    [Tooltip("Tag del jugador que puede activar la salida")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Verificar si complet� todos los objetivos
            if (ObjectiveManager.Instance != null && ObjectiveManager.Instance.AllComplete())
            {
                Debug.Log("�Todos los objetivos completados! Cargando escena de victoria...");
                SceneManager.LoadScene(victorySceneName);
            }
            else
            {
                Debug.Log("Todav�a faltan objetivos. No puedes salir.");
            }
        }
    }
}
