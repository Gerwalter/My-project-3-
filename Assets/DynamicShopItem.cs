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
    [SerializeField] private ItemDetails itemDetails; // Detalles del �tem
    private GoldManager _goldManager;

    private void Start()
    {
        // Busca la referencia al GoldManager
        _goldManager = FindObjectOfType<GoldManager>();

        if (_goldManager == null)
        {
            Debug.LogError("No se encontr� GoldManager en la escena.");
        }
    }

    /// <summary>
    /// Configura din�micamente los detalles del �tem desde el men�.
    /// </summary>
    /// <param name="details">Estructura con los detalles del �tem.</param>
    public void SetItemDetails(ItemDetails details)
    {
        itemDetails = details;
    }

    /// <summary>
    /// Llamado al hacer clic en el bot�n de este �tem.
    /// </summary>
    /// <returns>El nombre del �tem si la compra fue exitosa, o una cadena vac�a si fall�.</returns>
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
