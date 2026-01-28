using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ComboGraphVisualizer : MonoBehaviour
{
    [Header("References")]
    public ComboNode baseNode;
    public TextMeshProUGUI textMesh;

    [Header("Settings")]
    public int maxDepth = 10; // seguridad extra

    private StringBuilder builder;
    private HashSet<ComboNode> visitedNodes;

    void Start()
    {
        if (baseNode == null || textMesh == null)
        {
            Debug.LogWarning("Falta asignar baseNode o TextMeshPro");
            return;
        }

        BuildGraph();
    }

    [ContextMenu("Rebuild Graph")]
    public void BuildGraph()
    {
        builder = new StringBuilder();
        visitedNodes = new HashSet<ComboNode>();

        visitedNodes.Add(baseNode);

        // Empezamos directamente desde sus hijos
        DrawNode(baseNode, 0, 0);

        textMesh.text = builder.ToString();
    }

    private void DrawNode(ComboNode node, int indent, int depth)
    {
        if (depth >= maxDepth)
        {
            AppendIndented(indent, "<color=#ff5555>⛔ Límite de profundidad</color>");
            return;
        }

        foreach (var transition in node.transitions)
        {
            string inputName = transition.input.ToString();
            ComboNode next = transition.nextNode;

            if (next == null)
            {
                AppendIndented(indent, $"[<i>{inputName}</i>] → <color=gray>null</color>");
                continue;
            }

            // Flecha visual
            AppendIndented(
                indent,
                $"<color=#66ccff>{next.nodeName}</color> → [<b>{inputName}</b>]"
            );

            if (visitedNodes.Contains(next))
            {
                continue;
            }

            visitedNodes.Add(next);
            DrawNode(next, indent + 1, depth + 1);
        }
    }

    private void AppendIndented(int indent, string text)
    {
        builder.AppendLine($"{new string(' ', indent * 4)}• {text}");
    }
}