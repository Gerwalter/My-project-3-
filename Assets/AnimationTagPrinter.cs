using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTagPrinter : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {

        if (animator == null)
        {
            Debug.LogError("No se encontr� un componente Animator en este GameObject.");
        }
    }

    void Update()
    {
        if (animator == null) return;

        // Obtener el estado actual en la capa 0 (puedes cambiar la capa si es necesario)
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // Imprimir el nombre del estado de animaci�n actual
        Debug.Log("Animaci�n actual: " + currentState.fullPathHash);
    }
}