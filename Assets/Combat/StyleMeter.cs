using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StyleMeter : MonoBehaviour
{
    public TMP_Text styleText; // UI para mostrar la letra (puedes usar animaciones también)

    public float stylePoints = 0f;
    public float decayRate = 5f; // Qué tan rápido baja con el tiempo
    public float resetTime = 3f;
    private float timer;

    public StyleRank currentRank = StyleRank.D;
    [SerializeField] private readonly float[] rankThresholds = { 0, 100, 200, 300, 500, 700, 1000, Mathf.Infinity };
    public Image styleBar;


    [Header("Multiplicador de Estilo")]
    public float styleMultiplier = 1f;
    public float maxMultiplier = 5f;
    public float multiplierGrowthRate = 0.5f; // Cuánto sube por segundo mientras mantienes el combo
    public float multiplierDecayRate = 1f;    // Cuánto baja por segundo si no atacas
    public float decayAmplification = 5f;     // Multiplica el decaimiento normal por el multiplicador

    private void Awake()
    {
        EventManager.Subscribe("Reset", Resetter);
    }

    public void Resetter(params object[] args)
    {
        ResetStyle();
    }

    private void Start()
    {
        UpdateRank();
    }

    void IncreaseMultiplier()
    {
        styleMultiplier += multiplierGrowthRate * Time.deltaTime;
        styleMultiplier = Mathf.Clamp(styleMultiplier, 1f, maxMultiplier);
    }

    void Update()
    {
        StylePoints();
    }

    private void StylePoints()
    {
        timer += Time.deltaTime;
        if (stylePoints == 0) 
        {
            styleText.enabled = false;
            styleBar.enabled = false;
        }
        else
        {
            styleText.enabled = true;
            styleBar.enabled = true;
        }

        if (timer >= resetTime)
        {
            float amplifiedDecay = decayRate * (1f + styleMultiplier * decayAmplification);
            stylePoints = Mathf.Max(0f, stylePoints - amplifiedDecay * Time.deltaTime);
            UpdateRank();
            UpdateBar();
        }
        styleText.color = GetColorForRank(currentRank);
        styleBar.color = GetColorForRank(currentRank);
        
    }

    Color GetColorForRank(StyleRank rank)
    {
        switch (rank)
        {
            case StyleRank.D: return Color.gray;
            case StyleRank.C: return Color.white;
            case StyleRank.B: return Color.green;
            case StyleRank.A: return Color.cyan;
            case StyleRank.S: return Color.magenta;
            case StyleRank.SS: return Color.yellow;
            case StyleRank.SSS: return Color.red;
            default: return Color.white;
        }
    }

    void UpdateBar()
    {
        if (styleBar == null) return;

        int currentIndex = (int)currentRank;
        float currentMin = rankThresholds[currentIndex];
        float nextThreshold = rankThresholds[currentIndex + 1];

        float progress = (stylePoints - currentMin) / (nextThreshold - currentMin);
        styleBar.fillAmount = progress;
    }


    public void AddStylePoints(float amount)
    {
        stylePoints += amount * styleMultiplier;
        timer = 0f;
        UpdateRank();
        UpdateBar();
        IncreaseMultiplier();
    }

    public void ResetStyle()
    {
        stylePoints = 0f;
        timer = 0f;
        UpdateRank();
        UpdateBar();
    }

    void UpdateRank()
    {
        StyleRank previousRank = currentRank;

        if (stylePoints >= 1000f) currentRank = StyleRank.SSS;
        else if (stylePoints >= 700f) currentRank = StyleRank.SS;
        else if (stylePoints >= 500f) currentRank = StyleRank.S;
        else if (stylePoints >= 300f) currentRank = StyleRank.A;
        else if (stylePoints >= 200f) currentRank = StyleRank.B;
        else if (stylePoints >= 100f) currentRank = StyleRank.C;
        else currentRank = StyleRank.D;

        if (styleText != null)
            styleText.text = currentRank.ToString();

        // Opcional: log o feedback
        if (currentRank != previousRank)
        {
            Debug.Log("Rango de estilo cambiado a: " + currentRank);
            // Aquí puedes hacer efectos visuales, sonidos, animaciones, etc.
        }
    }
}
public enum StyleRank
{
    D,
    C,
    B,
    A,
    S,
    SS,
    SSS
}