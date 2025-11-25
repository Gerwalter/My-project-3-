using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger_Start : MonoBehaviour
{

    [Header("Dialogue Sequence")]
    [SerializeField]
    private List<DialogueLine> dialogueLines = new List<DialogueLine>()
    {
        new DialogueLine { text = "Primera línea del cartel..."},
        new DialogueLine { text = "Segunda línea (avanzas con Espacio)."},
        new DialogueLine { text = "¡Y la última!"}
    };
    [Header("One-Time Settings")]
    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private string uniqueID = "";

    [Header("Safety Delay")]
    [SerializeField] private float startDelay = 0.5f; // Segundos de gracia al iniciar escena

    private bool hasPlayed = false;
    private bool canTrigger = false; // <-- Bloquea triggers al inicio
    private static readonly string SAVE_KEY = "DialoguePlayed_";

    private void Awake()
    {
        // Activar detección segura después de X segundos
        Invoke(nameof(EnableTrigger), startDelay);
    }

    private void EnableTrigger()
    {
        canTrigger = true;
    }

    public void Interact()
    {
        if (!canTrigger) return;
        if (hasPlayed) return;

        EventManager.Trigger("StartDialogueSequence", dialogueLines);

        if (playOnlyOnce)
        {
            hasPlayed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;           // ← Protección principal
        if (!other.CompareTag("Player")) return;

        Interact();
    }

    // Opcional: también bloqueamos OnTriggerStay por si acaso
    private void OnTriggerStay(Collider other)
    {
        if (!canTrigger) return;
        if (!other.CompareTag("Player")) return;

        // Si quieres que se active al quedarse dentro después del delay
        // (raro, pero por completitud)
        Interact();
    }

    // Para pruebas: reset manual
    [ContextMenu("Reset One-Time Flag")]
    private void ResetPlayedFlag()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY + uniqueID);
        hasPlayed = false;
        Debug.Log($"[DialogueTrigger] Reset flag para {uniqueID}");
    }
}
