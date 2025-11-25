using TMPro;
using UnityEngine;

public class ScoreSummaryUI : MonoBehaviour, IPointObserver
{
    [Header("TextMeshPro Reference (ÚNICO)")]
    [SerializeField] private TextMeshProUGUI textSummary;

    [Header("Puntos por tipo de ítem (puedes ajustar)")]
    [SerializeField] private int pointsPerVasija = 50;
    [SerializeField] private int pointsPerCuadro = 100;
    [SerializeField] private int pointsPerArtefacto = 150;

    private int totalScore = 0;

    private void Start()
    {
        // Nos suscribimos al sistema de puntos general (opcional)
        ThiefPointSystem.instance?.Subscribe(this);

        // Actualizar al inicio
        UpdateScoreDisplay();
    }

    private void OnEnable()
    {
        // Suscribirse a actualizaciones de objetivos
        EventManager.Subscribe("ObjectivesUpdated", OnObjectivesUpdated);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe("ObjectivesUpdated", OnObjectivesUpdated);
        ThiefPointSystem.instance?.Unsubscribe(this);
    }

    // Se llama cada vez que cambia algún objetivo
    private void OnObjectivesUpdated(params object[] param)
    {
        UpdateScoreDisplay();
    }

    // Implementación de IPointObserver (opcional)
    public void Notify(float currentPoints)
    {
        // Ignoramos por ahora, ya que usamos solo ítems robados
    }

    private void UpdateScoreDisplay()
    {
        if (ObjectiveManager.Instance == null || textSummary == null)
        {
            Debug.LogWarning("ObjectiveManager o TextSummary no encontrado!");
            return;
        }

        int vasijas = ObjectiveManager.Instance.GetCurrentCount(ItemType.Vasija);
        int cuadros = ObjectiveManager.Instance.GetCurrentCount(ItemType.Cuadro);
        int artefactos = ObjectiveManager.Instance.GetCurrentCount(ItemType.Artefacto);

        int scoreVasijas = vasijas * pointsPerVasija;
        int scoreCuadros = cuadros * pointsPerCuadro;
        int scoreArtefactos = artefactos * pointsPerArtefacto;

        totalScore = scoreVasijas + scoreCuadros + scoreArtefactos;

        // Construir texto completo con formato Rich Text (colores y tamaños)
        string summaryText = $"<color=#FFD700>Vasijas: <b>{vasijas}</b> × {pointsPerVasija} = <color=#00FF00>{scoreVasijas}</color> pts</color>\n" +
                            $"<color=#FFD700>Cuadros: <b>{cuadros}</b> × {pointsPerCuadro} = <color=#00FF00>{scoreCuadros}</color> pts</color>\n" +
                            $"<color=#FFD700>Artefactos: <b>{artefactos}</b> × {pointsPerArtefacto} = <color=#00FF00>{scoreArtefactos}</color> pts</color>\n" +
                            $"<size=130%><color=#FF4500>TOTAL: <b>{totalScore}</b> puntos</color></size>";

        textSummary.text = summaryText;
    }

    // Método público para obtener el puntaje total
    public int GetTotalScore() => totalScore;
}