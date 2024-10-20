using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public enum ComboInput { None, LightAttack, HeavyAttack, Wait }

    [System.Serializable]
    public struct ComboSequence
    {
        public ComboInput[] sequence;
        public string comboName;
        public float waitTimeBeforeNextInput;
    }

    public ComboSequence[] comboSequences;
    public float inputWindowTime = 1.0f;          // Tiempo inicial para registrar inputs
    public float additionalTimePerInput = 0.5f;   // Tiempo extra por cada nuevo input
    public int maxInputs = 7;                     // Máximo de inputs permitidos en la secuencia
    private float inputWindowTimer = 0f;          // Temporizador para la ventana de inputs
    private bool isWaitingForInput = false;       // Indica si estamos esperando inputs dentro del tiempo permitido

    private List<ComboInput> currentCombo = new List<ComboInput>();
    private bool isAttacking = false;

    // Referencia a TextMeshProUGUI para mostrar los inputs en pantalla
    public TextMeshProUGUI inputDisplay;

    // Referencia al Animator
    public Animator animator;

    void Start()
    {
        UpdateInputDisplay();
    }

    void Update()
    {
        // Si hay inputs en progreso, inicia el temporizador
        if (isWaitingForInput)
        {
            inputWindowTimer += Time.deltaTime;

            // Si se ha terminado el tiempo de espera, verificamos los inputs
            if (inputWindowTimer >= inputWindowTime)
            {
                CheckCombo();
                ResetCombo();
            }
        }

        // Revisamos si se presiona alguno de los botones de ataque
        if (Input.GetMouseButtonDown(0)) // Mouse0 para Light Attack
        {
            RegisterInput(ComboInput.LightAttack);
            animator.SetTrigger("LightAttack");  // Disparar el trigger LightAttack en el Animator
        }
        if (Input.GetMouseButtonDown(1)) // Mouse1 para Heavy Attack
        {
            RegisterInput(ComboInput.HeavyAttack);
            animator.SetTrigger("HeavyAttack"); // Disparar el trigger HeavyAttack en el Animator
        }
    }

    void RegisterInput(ComboInput input)
    {
        if (isAttacking) return; // No registrar si ya está atacando

        // Si es el primer input, comenzamos la ventana de tiempo
        if (!isWaitingForInput)
        {
            isWaitingForInput = true;
            inputWindowTimer = 0f;
        }

        // Añadimos el input a la lista
        currentCombo.Add(input);

        // Cada vez que se registra un input, añadimos tiempo extra al temporizador
        inputWindowTimer = Mathf.Max(0f, inputWindowTimer - additionalTimePerInput);

        // Si alcanzamos el número máximo de inputs, forzamos el fin del temporizador
        if (currentCombo.Count >= maxInputs)
        {
            inputWindowTimer = inputWindowTime; // Forzamos que el temporizador expire
        }

        UpdateInputDisplay(); // Actualizamos el texto en la UI
    }

    void CheckCombo()
    {
        // Checamos todas las secuencias posibles y buscamos la que coincida exactamente con los inputs
        foreach (var combo in comboSequences)
        {
            if (ComboMatches(combo.sequence))
            {
                StartCoroutine(PerformCombo(combo.comboName));
                return;  // Ejecutamos solo el primer combo que coincida
            }
        }

        // Si no se encuentra ningún combo, solo limpiamos la secuencia
        Debug.Log("No combo found for this sequence");
    }

    bool ComboMatches(ComboInput[] sequence)
    {
        // El combo debe tener la misma longitud exacta para que coincida
        if (currentCombo.Count != sequence.Length) return false;

        for (int i = 0; i < currentCombo.Count; i++)
        {
            if (currentCombo[i] != sequence[i])
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator PerformCombo(string comboName)
    {
        isAttacking = true;
        Debug.Log("Combo ejecutado: " + comboName); // Imprimimos el nombre del combo en la consola
        yield return new WaitForSeconds(0.5f);      // Simulación de tiempo de ataque
        ResetCombo();                               // Limpiamos el combo después de ejecutar
        UpdateInputDisplay();                       // Actualizamos el texto en la UI
        isAttacking = false;
    }

    // Método para resetear el combo y la ventana de tiempo
    void ResetCombo()
    {
        currentCombo.Clear();      // Limpiamos la lista de inputs
        isWaitingForInput = false; // Detenemos la ventana de espera de inputs
        inputWindowTimer = 0f;     // Reiniciamos el temporizador
        UpdateInputDisplay();
    }

    // Método para actualizar el texto en la UI
    void UpdateInputDisplay()
    {
        if (inputDisplay != null)
        {
            if (currentCombo.Count == 0)
            {
                inputDisplay.text = "No Input";
            }
            else
            {
                inputDisplay.text = "Combo: ";
                foreach (var input in currentCombo)
                {
                    inputDisplay.text += input.ToString() + " ";
                }
            }
        }
    }
}
