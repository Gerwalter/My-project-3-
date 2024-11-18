using UnityEngine;
using UnityEngine.VFX;

public class ActivateVFX : MonoBehaviour
{
    // Referencia al componente VisualEffect
    [SerializeField] private VisualEffect[] vfxArray;

    // Tecla para activar el VFX
    [SerializeField] private KeyCode activationKey = KeyCode.Space;

    // Nombre del par�metro booleano en el VFX Graph, si es necesario
    [SerializeField] private string vfxParameter = "PlayVFX";

    private void Update()
    {
        // Detectar si se presiona la tecla asignada
        if (Input.GetKeyDown(activationKey))
        {
            PlayVFX();
        }
    }

    private void PlayVFX()
    {
       foreach (var vfx in vfxArray)
        {
            if (vfx != null)
            {
                // Si el VFX Graph tiene un par�metro booleano para activar

                
                    vfx.SetBool(vfxParameter, true);
                

                // Reiniciar el VFX para que las part�culas comiencen de nuevo
                vfx.Reinit();
            }
            else
            {
                Debug.LogWarning("Un VisualEffect no est� asignado en el array.");
            }
        }
    }
}
