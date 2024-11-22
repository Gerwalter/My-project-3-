using UnityEngine;
using UnityEngine.VFX;

public class ActivateVFX : MonoBehaviour
{
    // Referencia al componente VisualEffect
    [SerializeField] private VisualEffect[] vfxArray;

    // Nombre del par�metro booleano en el VFX Graph, si es necesario
    [SerializeField] private string vfxParameter = "PlayVFX";

    private void PlayVFX()
    {
        foreach (var vfx in vfxArray)
        {
            if (vfx != null)
            {
                vfx.SetBool(vfxParameter, true);
                vfx.Reinit();
            }
            else
            {
                Debug.LogWarning("Un VisualEffect no est� asignado en el array.");
            }
        }
    }
}
