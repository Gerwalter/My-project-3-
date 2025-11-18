using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI objectiveText; // Asigna el TextMeshProUGUI del Canvas
    public string nextSceneName = "MainLevel"; // Cambia por el nombre real de la escena principal

    private void Start()
    {
        UpdateObjectives();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        LoadingScreen.nextScene = nextSceneName;
        SceneManager.LoadScene("LoadingScene");
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1; // Asegura que el tiempo se reanude al salir
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu"); // Cambia "MainMenu" por el nombre de tu escena principal
    }
    private void UpdateObjectives()
    {
        if (ObjectiveManager.Instance == null || objectiveText == null)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("<b>Mission Objectives</b>\n");

        foreach (var cfg in ObjectiveManager.Instance.objectiveConfigs)
        {  // Si el objetivo requerido es 0, no lo mostramos
            if (cfg.requiredCount <= 0)
                continue;

            sb.AppendLine($"- {cfg.requiredCount} × {cfg.itemType}");
        }

        objectiveText.text = sb.ToString();
    }
}
