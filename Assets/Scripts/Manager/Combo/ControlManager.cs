using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq; // Para usar SequenceEqual

public class ControlManager : MonoBehaviour
{
    [SerializeField] float comboResetTime = 0.5f;
    List<KeyCode> keysPressed = new List<KeyCode>(); // Lista interna de teclas presionadas

    [SerializeField] TextMeshProUGUI controlsTestText;

    [SerializeField] List<Move> availableMoves; // Lista de todos los movimientos disponibles
    PlayerController playerController;
    Coroutine resetCoroutine;

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        availableMoves.Sort((m1, m2) => m2.GetMoveComboPriorty().CompareTo(m1.GetMoveComboPriorty()));
    }

    void Update()
    {
        DetectPressedKey();
        PrintControls(); // Solo para testing
    }

    public void DetectPressedKey()
    {
        // Detección para teclas del teclado
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                keysPressed.Add(key);
                if (resetCoroutine != null)
                    StopCoroutine(resetCoroutine);

                resetCoroutine = StartCoroutine(ResetComboTimer());

                Move matchedMove = GetMatchingMove(keysPressed);
                if (matchedMove != null)
                {
                    playerController.PlayMove(matchedMove);
                    ResetCombo();
                }
            }
        }

        // Detección específica para botones del mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            keysPressed.Add(KeyCode.Mouse0);
            if (resetCoroutine != null)
                StopCoroutine(resetCoroutine);

            resetCoroutine = StartCoroutine(ResetComboTimer());

            Move matchedMove = GetMatchingMove(keysPressed);
            if (matchedMove != null)
            {
                playerController.PlayMove(matchedMove);
                ResetCombo();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            keysPressed.Add(KeyCode.Mouse1);
            if (resetCoroutine != null)
                StopCoroutine(resetCoroutine);

            resetCoroutine = StartCoroutine(ResetComboTimer());

            Move matchedMove = GetMatchingMove(keysPressed);
            if (matchedMove != null)
            {
                playerController.PlayMove(matchedMove);
                ResetCombo();
            }
        }
    }

    IEnumerator ResetComboTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        ResetCombo();
    }

    void ResetCombo()
    {
        keysPressed.Clear();
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    Move GetMatchingMove(List<KeyCode> keyCodes)
    {
        foreach (Move move in availableMoves)
        {
            if (move.IsMoveAvailable(keyCodes))
                return move;
        }
        return null;
    }

    void PrintControls() // Solo para testing
    {
        controlsTestText.text = "Keys Pressed: " + string.Join(", ", keysPressed);
    }
}
