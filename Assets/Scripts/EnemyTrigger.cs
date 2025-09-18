using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "BattleScene"; // Nombre de la escena de pelea

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Guardamos la posición actual del jugador
            Vector3 playerPos = other.transform.position;
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // Iniciar batalla
            BattleManager.Instance.StartBattle(playerPos, currentScene, battleSceneName);
        }
    }
}
