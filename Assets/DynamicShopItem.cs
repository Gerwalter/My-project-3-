using UnityEngine;

[System.Serializable]
public struct ItemDetails
{
    public int cost;
    public string name;

    public ItemDetails(int cost, string name)
    {
        this.cost = cost;
        this.name = name;
    }
}

public class DynamicShopItem : MonoBehaviour
{
    [SerializeField] private ItemDetails itemDetails; // Detalles del ítem
    private GoldManager _goldManager;

    private void Start()
    {
        // Busca la referencia al GoldManager
        _goldManager = FindObjectOfType<GoldManager>();

        if (_goldManager == null)
        {
            Debug.LogError("No se encontró GoldManager en la escena.");
        }
    }

    /// <summary>
    /// Configura dinámicamente los detalles del ítem desde el menú.
    /// </summary>
    /// <param name="details">Estructura con los detalles del ítem.</param>
    public void SetItemDetails(ItemDetails details)
    {
        itemDetails = details;
    }

    /// <summary>
    /// Llamado al hacer clic en el botón de este ítem.
    /// </summary>
    /// <returns>El nombre del ítem si la compra fue exitosa, o una cadena vacía si falló.</returns>
    public string PurchaseItem()
    {
        if (_goldManager == null) return "";

        if (_goldManager.SpendGold(itemDetails.cost))
        {
            Debug.Log($"Compra realizada: {itemDetails.name} por {itemDetails.cost} de oro.");
            return itemDetails.name;
        }

        Debug.Log("No tienes suficiente oro para comprar este objeto.");
        return "";
    }
}
