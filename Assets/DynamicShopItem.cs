using UnityEngine;


[System.Serializable]
public struct ItemDetails
{
    public int cost;
    public string name;
    public GameObject item;

    public ItemDetails(int cost, string name, GameObject item)
    {
        this.cost = cost;
        this.name = name;
        this.item = item;
    }
}

public class DynamicShopItem : MonoBehaviour
{
    [SerializeField] private ItemDetails[] itemDetails; // Detalles del ítem
    [SerializeField] private GoldManager _goldManager;
    public Transform spawn;
    public PPMenu menu;

    private void Start()
    {
        // Busca la referencia al GoldManager
        _goldManager = FindObjectOfType<GoldManager>();

        if (_goldManager == null)
        {
            Debug.LogError("No se encontró GoldManager en la escena.");
        }
    }

    public void SetItemDetails(ItemDetails[] details)
    {
        itemDetails = details;
    }

    public void PurchaseItem(string itemName)
    {
        ItemDetails? selectedItem = null;
        foreach (var item in itemDetails)
        {
            if (item.name == itemName)
            {
                selectedItem = item;
                break;
            }
        }

        if (_goldManager.SpendGold(selectedItem.Value.cost))
        {
            Vector3 spawnPosition = spawn.transform.position;
            Quaternion spawnRotation = Quaternion.identity;

            Instantiate(selectedItem.Value.item, spawnPosition, spawnRotation);
            menu.Menudisable();
        }
    }
}
