using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public Dictionary<string, KeyCode> inputs;

    public float forwardInput { get; private set; }
    public float rotationInput { get; private set; }
    public bool run { get; private set; }
    public bool jump { get; private set; }

    private void Start()
    {
        // Inicializar el diccionario de inputs
        inputs = new Dictionary<string, KeyCode>
        {
            {"forwardW", KeyCode.W},
            {"backwardS", KeyCode.S},
            {"leftA", KeyCode.A},
            {"rightD", KeyCode.D},
            {"forwardUp", KeyCode.UpArrow},
            {"backwardDown", KeyCode.DownArrow},
            {"leftLeft", KeyCode.LeftArrow},
            {"rightRight", KeyCode.RightArrow},
            {"runLeftShift", KeyCode.LeftShift},
            {"runRightShift", KeyCode.RightShift},
            {"jump", KeyCode.Space},
            {"altJump", KeyCode.RightControl}
        };
    }

    private void Update()
    {
        // Captura los inputs y almacena los valores necesarios
        forwardInput = Input.GetKey(inputs["forwardW"]) || Input.GetKey(inputs["forwardUp"]) ? 1 :
                       (Input.GetKey(inputs["backwardS"]) || Input.GetKey(inputs["backwardDown"]) ? -1 : 0);

        rotationInput = Input.GetKey(inputs["leftA"]) || Input.GetKey(inputs["leftLeft"]) ? -1 :
                        (Input.GetKey(inputs["rightD"]) || Input.GetKey(inputs["rightRight"]) ? 1 : 0);

        run = Input.GetKey(inputs["runLeftShift"]) || Input.GetKey(inputs["runRightShift"]);
        jump = Input.GetKeyDown(inputs["jump"]) || Input.GetKeyDown(inputs["altJump"]);
    }
}
