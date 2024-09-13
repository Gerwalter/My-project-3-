using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float _health;
    [SerializeField] float _maxhealth;

    private void Start()
    {
        _health = _maxhealth;
    }

    public void Damage()
    {
        if (_health <= 0)
        {
            _health = 0; // Aseguramos que la salud no sea negativa
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
