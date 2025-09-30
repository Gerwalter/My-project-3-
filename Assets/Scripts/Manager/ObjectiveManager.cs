using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Vasija, Cuadro, Artefacto }

[System.Serializable]
public class ObjectiveRandomConfig
{
    public ItemType itemType;
    public int minRequired = 1;
    public int maxRequired = 4;

    [HideInInspector] public int currentCount = 0;
    [HideInInspector] public int requiredCount = 0;

    public bool IsComplete => currentCount >= requiredCount;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Tooltip("Configura el rango de objetivos posibles para cada tipo")]
    public List<ObjectiveRandomConfig> objectiveConfigs = new List<ObjectiveRandomConfig>();

    public int valuePerItem = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Generar objetivos aleatorios al iniciar
        GenerateRandomObjectives();

        // Suscribirse para responder cuando el cofre pida valor
        EventManager.Subscribe("ObtainAlert", OnObtainAlert);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("ObtainAlert", OnObtainAlert);
    }

    private void GenerateRandomObjectives()
    {
        foreach (var cfg in objectiveConfigs)
        {
            cfg.currentCount = 0;
            cfg.requiredCount = Random.Range(cfg.minRequired, cfg.maxRequired + 1);
        }

        Debug.Log("Objetivos generados:");
        foreach (var cfg in objectiveConfigs)
        {
            Debug.Log($"{cfg.itemType} → {cfg.requiredCount}");
        }

        EventManager.Trigger("ObjectivesUpdated");
    }

    private void OnObtainAlert(params object[] parameters)
    {
        int totalValue = 0;
        foreach (var cfg in objectiveConfigs)
        {
            totalValue += cfg.currentCount * valuePerItem;
        }
        EventManager.Trigger("ReceiveAlertValue", totalValue);
    }

    public void Steal(ItemType type, int amount = 1)
    {
        var cfg = objectiveConfigs.Find(o => o.itemType == type);
        if (cfg != null)
        {
            cfg.currentCount += amount;
            Debug.Log($"Robado {amount}x {type}. Ahora {cfg.currentCount}/{cfg.requiredCount}");
            EventManager.Trigger("ObjectivesUpdated");
        }
    }

    public int GetCurrentCount(ItemType type)
    {
        var cfg = objectiveConfigs.Find(o => o.itemType == type);
        return cfg != null ? cfg.currentCount : 0;
    }

    public int GetRequiredCount(ItemType type)
    {
        var cfg = objectiveConfigs.Find(o => o.itemType == type);
        return cfg != null ? cfg.requiredCount : 0;
    }

    public bool AllComplete()
    {
        foreach (var cfg in objectiveConfigs) if (!cfg.IsComplete) return false;
        return true;
    }
}
