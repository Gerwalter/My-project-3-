using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StyleMeter : MonoBehaviour
{
    [Header("UI – Rank Sprite")]
    public Image rankImage;
    public Sprite spriteD;
    public Sprite spriteC;
    public Sprite spriteB;
    public Sprite spriteA;
    public Sprite spriteS;
    public Sprite spriteSS;
    public Sprite spriteSSS;

    [Header("UI – Bar Sprites por Rango")]
    public Sprite barD;
    public Sprite barC;
    public Sprite barB;
    public Sprite barA;
    public Sprite barS;  // Compartida para S – SS – SSS

    [Header("UI – Marco Sprites por Rango")]
    public Sprite frameD;
    public Sprite frameC;
    public Sprite frameB;
    public Sprite frameA;
    public Sprite frameS;  // Compartida para S – SS – SSS

    public Image styleBar;
    public Image PanelBar;

    [Header("Sistema de Estilo")]
    public float stylePoints = 0f;
    public float decayRate = 5f;
    public float resetTime = 3f;
    private float timer;

    public StyleRank currentRank = StyleRank.D;
    [SerializeField] private readonly float[] rankThresholds = { 0, 100, 200, 300, 500, 700, 1000, Mathf.Infinity };

    [Header("Multiplicador")]
    public float styleMultiplier = 1f;
    public float maxMultiplier = 5f;
    public float multiplierGrowthRate = 0.5f;
    public float multiplierDecayRate = 1f;
    public float decayAmplification = 5f;

    private void Awake()
    {
        EventManager.Subscribe("Reset", Resetter);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        EventManager.Unsubscribe("Reset", Resetter);
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

        bool activeUI = stylePoints > 0;

        if (rankImage != null) rankImage.enabled = activeUI;
        if (styleBar != null) styleBar.enabled = activeUI;
        if (PanelBar != null) PanelBar.enabled = activeUI;

        if (timer >= resetTime)
        {
            float amplifiedDecay = decayRate * (1f + styleMultiplier * decayAmplification);
            stylePoints = Mathf.Max(0f, stylePoints - amplifiedDecay * Time.deltaTime);
            UpdateRank();
            UpdateBarFill();
        }

   //     if (styleBar != null)
    //        styleBar.color = GetColorForRank(currentRank);
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
        }
        return Color.white;
    }

    void UpdateBarFill()
    {
        if (styleBar == null) return;

        int currentIndex = (int)currentRank;
        float min = rankThresholds[currentIndex];
        float next = rankThresholds[currentIndex + 1];

        float progress = (stylePoints - min) / (next - min);
        styleBar.fillAmount = progress;
    }

    public void AddStylePoints(float amount)
    {
        stylePoints += amount * styleMultiplier;
        timer = 0f;

        UpdateRank();
        UpdateBarFill();
        IncreaseMultiplier();
    }

    public void ResetStyle()
    {
        stylePoints = 0f;
        timer = 0f;
        UpdateRank();
        UpdateBarFill();
    }

    void UpdateRank()
    {
        StyleRank previousRank = currentRank;

        if (stylePoints >= 1000) currentRank = StyleRank.SSS;
        else if (stylePoints >= 700) currentRank = StyleRank.SS;
        else if (stylePoints >= 500) currentRank = StyleRank.S;
        else if (stylePoints >= 300) currentRank = StyleRank.A;
        else if (stylePoints >= 200) currentRank = StyleRank.B;
        else if (stylePoints >= 100) currentRank = StyleRank.C;
        else currentRank = StyleRank.D;

        UpdateRankSprite();
        UpdateBarSprite();
        UpdateFrameSprite();
    }

    void UpdateRankSprite()
    {
        if (rankImage == null) return;

        switch (currentRank)
        {
            case StyleRank.D: rankImage.sprite = spriteD; break;
            case StyleRank.C: rankImage.sprite = spriteC; break;
            case StyleRank.B: rankImage.sprite = spriteB; break;
            case StyleRank.A: rankImage.sprite = spriteA; break;
            case StyleRank.S: rankImage.sprite = spriteS; break;
            case StyleRank.SS: rankImage.sprite = spriteSS; break;
            case StyleRank.SSS: rankImage.sprite = spriteSSS; break;
        }
    }

    void UpdateBarSprite()
    {
        if (styleBar == null) return;

        switch (currentRank)
        {
            case StyleRank.D: styleBar.sprite = barD; break;
            case StyleRank.C: styleBar.sprite = barC; break;
            case StyleRank.B: styleBar.sprite = barB; break;
            case StyleRank.A: styleBar.sprite = barA; break;

            // S – SS – SSS COMPARTEN LA MISMA BARRA
            case StyleRank.S:
            case StyleRank.SS:
            case StyleRank.SSS:
                styleBar.sprite = barS;
                break;
        }
    }

    void UpdateFrameSprite()
    {
        if (PanelBar == null) return;

        switch (currentRank)
        {
            case StyleRank.D: PanelBar.sprite = frameD; break;
            case StyleRank.C: PanelBar.sprite = frameC; break;
            case StyleRank.B: PanelBar.sprite = frameB; break;
            case StyleRank.A: PanelBar.sprite = frameA; break;

            // S – SS – SSS COMPARTEN MARCO
            case StyleRank.S:
            case StyleRank.SS:
            case StyleRank.SSS:
                PanelBar.sprite = frameS;
                break;
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
