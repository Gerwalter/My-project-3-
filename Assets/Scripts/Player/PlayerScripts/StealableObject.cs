using UnityEngine;
using UnityEngine.UI;

public class StealableObject : ButtonBehaviour
{
    public ItemType itemType = ItemType.Vasija;
    public int stealAmount = 1; // cuantos cuenta un robo (por defecto 1)
    public int alertAmmount = 1; // cuantos cuenta un robo (por defecto 1)
    public GameObject stealConfirmHUDPrefab; // opcional: un prefab con botones "Robar / Cancelar"
    public bool destroyOnSteal = true; // si se destruye el objeto al robar

    private GameObject activeHUD;

    public override void OnInteract()
    {
        // Si diste prefab para confirmación, lo instanciamos; si no, robamos directo
        if (stealConfirmHUDPrefab != null)
        {
            ShowConfirmHUD();
        }
        else
        {
            DoSteal();
        }
    }

    private void ShowConfirmHUD()
    {
        if (activeHUD != null) return;
        activeHUD = Instantiate(stealConfirmHUDPrefab, transform.position, Quaternion.identity);
        // asume que el prefab tiene botones con nombres "BtnSteal" y "BtnCancel"
        var btnSteal = activeHUD.transform.Find("BtnSteal")?.GetComponent<Button>();
        var btnCancel = activeHUD.transform.Find("BtnCancel")?.GetComponent<Button>();
        if (btnSteal != null) btnSteal.onClick.AddListener(() => { DoSteal(); Destroy(activeHUD); });
        if (btnCancel != null) btnCancel.onClick.AddListener(() => { Destroy(activeHUD); });
        // Si tu HUD es parte de Canvas, en vez de position preferirás instanciar como hijo del canvas.
    }

    private void DoSteal()
    {
        if (ObjectiveManager.Instance == null)
        {
            Debug.LogError("No hay ObjectiveManager en la escena.");
            return;
        }

        ObjectiveManager.Instance.Steal(itemType, stealAmount);
        EventManager.Trigger("IncreaseAlert", alertAmmount);
        // Aquí puedes reproducir sonido, animación, spawn de loot, etc.
        Debug.Log($"Objeto {itemType} robado.");

        if (destroyOnSteal)
        {
            Destroy(gameObject);
        }
    }
}
