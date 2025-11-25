using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : ButtonBehaviour
{

    [Header("Dialogue Sequence")]
    [SerializeField]
    private List<DialogueLine> dialogueLines = new List<DialogueLine>()
    {
        new DialogueLine { text = "Primera línea del cartel..."},
        new DialogueLine { text = "Segunda línea (avanzas con Espacio)."},
        new DialogueLine { text = "¡Y la última!"}
    };

    public override void OnInteract()
    {
        EventManager.Trigger("StartDialogueSequence", dialogueLines);
    }

}
[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 8)] public string text = "Escribe aquí el diálogo...";
    public bool useTypewriter = true;
    [Range(0f, 0.2f)] public float typeSpeedOverride = 0f; // 0 = usa el del DialogueManager
}