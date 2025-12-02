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
    [Header("COLORES (Editable en Inspector)")]
    [SerializeField] private Color colorLabel = Color.yellow;
    [SerializeField] private Color colorValue = Color.green;
    [SerializeField] private Color colorTotal = new Color(1f, 0.27f, 0f);  // Naranja
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

        // Convertir colores a hex
        string cLabel = ColorUtility.ToHtmlStringRGB(colorLabel);
        string cValue = ColorUtility.ToHtmlStringRGB(colorValue);
        string cTotal = ColorUtility.ToHtmlStringRGB(colorTotal);

        string summaryText =
            $"<color=#{cLabel}>Vasijas: <b>{vasijas}</b> × {pointsPerVasija} = <color=#{cValue}>{scoreVasijas}</color> pts</color>\n" +
            $"<color=#{cLabel}>Cuadros: <b>{cuadros}</b> × {pointsPerCuadro} = <color=#{cValue}>{scoreCuadros}</color> pts</color>\n" +
            $"<color=#{cLabel}>Artefactos: <b>{artefactos}</b> × {pointsPerArtefacto} = <color=#{cValue}>{scoreArtefactos}</color> pts</color>\n" +
            $"<size=130%><color=#{cTotal}>TOTAL: <b>{totalScore}</b> puntos</color></size>";

        textSummary.text = summaryText;
    }

    // Método público para obtener el puntaje total
    public int GetTotalScore() => totalScore;
}