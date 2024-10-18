using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para usar SequenceEqual

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public class Move : ScriptableObject
{
    [SerializeField] List<KeyCode> movesKeyCodes; // La secuencia de teclas para el movimiento
    [SerializeField] Moves moveType; // El tipo de movimiento
    [SerializeField] int comboPriority; // Prioridad del combo

    public bool IsMoveAvailable(List<KeyCode> playerKeyCodes)
    {
        // Compara las secuencias para verificar si coinciden
        return playerKeyCodes.Count >= movesKeyCodes.Count &&
               playerKeyCodes.GetRange(playerKeyCodes.Count - movesKeyCodes.Count, movesKeyCodes.Count)
                             .SequenceEqual(movesKeyCodes);
    }

    public int GetMoveComboPriorty()
    {
        return comboPriority;
    }

    public Moves GetMove()
    {
        return moveType;
    }
}
