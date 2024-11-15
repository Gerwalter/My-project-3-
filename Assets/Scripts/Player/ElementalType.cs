using UnityEngine;

public class ElementalType : MonoBehaviour
{
    [SerializeField] private ElementType selectedElement;
    private enum ElementType
    {
        Normal,
        Fire,
        Electric,
        Ice,
        Water,
    }

    public void ElementalCast()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            selectedElement = (ElementType)Random.Range(0, System.Enum.GetValues(typeof(ElementType)).Length);
            Debug.Log("Elemento seleccionado: " + selectedElement);
        }
    }
}
