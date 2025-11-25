using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI textUI;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private KeyCode advanceKey = KeyCode.Space;
    [SerializeField] private PlayerController playerMovement;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool sequenceActive = false;

    private void OnEnable()
    {
        EventManager.Subscribe("ShowDialogue", OnShowSingle);           // Retrocompatibilidad
        EventManager.Subscribe("StartDialogueSequence", OnStartSequence);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe("ShowDialogue", OnShowSingle);
        EventManager.Unsubscribe("StartDialogueSequence", OnStartSequence);
    }

    private void Update()
    {
        if (!dialoguePanel.activeInHierarchy) return;

        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else if (dialogueQueue.Count > 0)
            {
                ShowNextLine();
            }
            else
            {
                EndSequence();
            }
        }
    }

    // === Secuencia completa ===
    private void OnStartSequence(params object[] args)
    {
        if (sequenceActive) return;

        var lines = args[0] as List<DialogueLine>;
        bool oneTime = args.Length > 1 && (bool)args[1];

        dialogueQueue.Clear();
        foreach (var line in lines)
            dialogueQueue.Enqueue(line);

        sequenceActive = true;
        DisablePlayer();
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndSequence();
            return;
        }

        var line = dialogueQueue.Dequeue();
        float speed = line.typeSpeedOverride > 0 ? line.typeSpeedOverride : typeSpeed;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (line.useTypewriter)
            typingCoroutine = StartCoroutine(TypeText(line.text, speed));
        else
        {
            textUI.text = line.text;
            isTyping = false;
        }
    }

    private IEnumerator TypeText(string text, float speed)
    {
        isTyping = true;
        textUI.text = "";
        foreach (char c in text)
        {
            textUI.text += c;
            yield return new WaitForSeconds(speed);
        }
        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        // Recuperar el texto completo (lo guardamos temporalmente o usamos el último de la cola)
        textUI.text = dialogueQueue.Count > 0 ? dialogueQueue.Peek().text : "";
        isTyping = false;
    }

    private void EndSequence()
    {
        dialoguePanel.SetActive(false);
        sequenceActive = false;
        EnablePlayer();
    }

    // === Retrocompatibilidad (una sola línea) ===
    private void OnShowSingle(params object[] args)
    {
        string text = args[0]?.ToString() ?? "";
        bool instant = args.Length > 1 && (bool)args[1];

        var singleLine = new DialogueLine { text = text, useTypewriter = !instant };
        var list = new List<DialogueLine> { singleLine };

        EventManager.Trigger("StartDialogueSequence", list, false, "single");
    }

    private void DisablePlayer() => playerMovement.enabled = false;
    private void EnablePlayer() => playerMovement.enabled = true;
}