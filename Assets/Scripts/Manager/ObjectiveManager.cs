using System.Collections.Generic;
using UnityEngine;

// Enum con tipos de objetos robables
public enum ItemType { Vasija, Cuadro, Estatua }

[System.Serializable]
public class Objective
{
    public ItemType itemType;
    public int requiredCount = 1;
    [HideInInspector] public int currentCount = 0;

    public bool IsComplete => currentCount >= requiredCount;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Tooltip("Lista de objetivos (ej: 3 vasijas, 2 cuadros...)")]
    public List<Objective> objectives = new List<Objective>();

    // Valor por item (si quieres ponderar distinto)
    public int valuePerItem = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Suscribirse para responder cuando un cofre pida obtener el valor actual
        EventManager.Subscribe("ObtainAlert", OnObtainAlert);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("ObtainAlert", OnObtainAlert);
    }

    // Devuelve el "valor" total actual (suma de items * valuePerItem)
    private void OnObtainAlert(params object[] parameters)
    {
        int totalValue = 0;
        foreach (var obj in objectives)
        {
            totalValue += obj.currentCount * valuePerItem;
        }

        // Disparar evento con el valor para que lo reciba el cofre (tu ChestScript espera "ReceiveAlertValue")
        EventManager.Trigger("ReceiveAlertValue", totalValue);
    }

    // Llamar para marcar que se robó un item
    public void Steal(ItemType type, int amount = 1)
    {
        var obj = objectives.Find(o => o.itemType == type);
        if (obj != null)
        {
            obj.currentCount += amount;
            if (obj.currentCount > obj.requiredCount) print("No pa");//obj.currentCount = obj.currentCount; // permitimos sobrepasar si quieres
            Debug.Log($"Robado {amount}x {type}. Ahora {obj.currentCount}/{obj.requiredCount}");
            // notificar UI
            EventManager.Trigger("ObjectivesUpdated");
        }
        else
        {
            Debug.LogWarning($"No hay objetivo definido para {type}. Puedes añadirlo en ObjectiveManager.");
        }
    }

    // Consultas útiles
    public int GetCurrentCount(ItemType type)
    {
        var obj = objectives.Find(o => o.itemType == type);
        return obj != null ? obj.currentCount : 0;
    }

    public int GetRequiredCount(ItemType type)
    {
        var obj = objectives.Find(o => o.itemType == type);
        return obj != null ? obj.requiredCount : 0;
    }

    public bool AllComplete()
    {
        foreach (var o in objectives) if (!o.IsComplete) return false;
        return true;
    }

    // Reset opcional
    public void ResetAll()
    {
        foreach (var o in objectives) o.currentCount = 0;
        EventManager.Trigger("ObjectivesUpdated");
    }
}
