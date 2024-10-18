using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ControlManager controlManager;

    void Awake()
    {
        controlManager = FindObjectOfType<ControlManager>();
    }

    public void PlayMove(Move move) // Recibe directamente un objeto Move
    {
        if (move == null || move.GetMove() == Moves.None)
            return;

        Debug.Log($"Executing move: {move.GetMove()}");
    }
}
