using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawer : MonoBehaviour
{
    [SerializeField] private CoinFactory coinFactory; // Arrástralo en el Inspector
    [SerializeField] private Transform spawnPoint;    // Punto donde aparecerán las monedas
    [SerializeField] private int minCoins = 1;        // Mínimo a spawnear
    [SerializeField] private int maxCoins = 5;        // Máximo a spawnear
    [SerializeField] private float spawnRadius = 1f;
    private void Start()
    {
        // Ejemplo: crear monedas distintas en Start
     //   SpawnCoin("GoldCoin");
       // SpawnCoin("SilverCoin");
        //SpawnCoin("BronzeCoin");
    }

    public void SpawnCoin(string coinID)
    {
        Coin coin = coinFactory.Create(coinID);
        if (coin != null)
        {
            coin.transform.position = spawnPoint.position + new Vector3 (Random.Range(.5f, 4), Random.Range(.5f, 4), Random.Range(.5f, 4));
            Debug.Log("Se creó una moneda de tipo: " + coinID);
        }
    }
    public void SpawnRandomCoins()
    {
        if (coinFactory == null)
        {
            Debug.LogError("CoinSpawner: No se asignó CoinFactory");
            return;
        }

        // Elegir cantidad aleatoria
        int amount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < amount; i++)
        {
            // Obtener un ID random de las monedas del factory
            string randomID = coinFactory.GetRandomCoinID();

            // Crear la moneda
            Coin coin = coinFactory.Create(randomID);

            if (coin != null)
            {
                // Colocar la moneda en una posición aleatoria alrededor del spawnPoint
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0; // Para que no aparezcan flotando
                coin.transform.position = spawnPoint.position + randomOffset;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
