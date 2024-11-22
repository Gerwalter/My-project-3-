using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private int itemCost = 30;
    [SerializeField] private string itemName;  
    private GoldManager _goldManager;
    public GameObject apple;
    public Transform spawn;

    private void Start()
    {
        _goldManager = FindObjectOfType<GoldManager>();

        if (_goldManager == null)
        {
            Debug.LogError("No se encontró GoldManager en la escena.");
        }
    }


    public void PurchaseItem()
    {
        if (_goldManager == null) return;

        if (_goldManager.SpendGold(itemCost))
        {
            // Determinar la posición frente al objeto
            Vector3 spawnPosition = spawn.transform.position; // Cambia la distancia si es necesario
            Quaternion spawnRotation = Quaternion.identity; // Rotación por defecto

            // Instanciar el objeto apple en la posición calculada
            Instantiate(apple, spawnPosition, spawnRotation);

            // Aquí puedes añadir lógica para dar el objeto al jugador
        }
        else
        {
            Debug.Log("No tienes suficiente oro para comprar este objeto.");
        }
    }

    public void SetItemDetails(int cost, string name)
    {
        itemCost = cost;
        itemName = name;
    }
}
