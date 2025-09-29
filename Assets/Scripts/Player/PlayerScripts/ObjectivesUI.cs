using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ObjectivesUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject rowPrefab; // un GameObject con un Text (o TMP) para mostrar "Vasija 1/3"
    public RectTransform contentParent;

    private Dictionary<ItemType, TextMeshProUGUI> rows = new Dictionary<ItemType, TextMeshProUGUI>();

    private void Start()
    {
        RefreshUI();
        EventManager.Subscribe("ObjectivesUpdated", OnObjectivesUpdated);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("ObjectivesUpdated", OnObjectivesUpdated);
    }
    private void Update()
    {
        if (panel != null)
        {
            // Mostrar mientras Tab está presionado
            panel.SetActive(Input.GetKey(KeyCode.Tab));
        }
    }

    private void OnObjectivesUpdated(params object[] p) => RefreshUI();

    public void RefreshUI()
    {
        foreach (Transform t in contentParent) Destroy(t.gameObject);
        rows.Clear();

        if (ObjectiveManager.Instance == null) return;

        foreach (var cfg in ObjectiveManager.Instance.objectiveConfigs)
        {
            //  Aquí filtramos: si el objetivo tiene requiredCount = 0, no lo mostramos
            if (cfg.requiredCount <= 0) continue;

            GameObject row = Instantiate(rowPrefab, contentParent);
            TextMeshProUGUI t = row.GetComponentInChildren<TextMeshProUGUI>();
            if (t != null)
            {
                t.text = $"{cfg.itemType} {cfg.currentCount}/{cfg.requiredCount}";
                rows[cfg.itemType] = t;
            }
        }
    }

    public void UpdateRow(ItemType type)
    {
        if (ObjectiveManager.Instance == null) return;
        var cfg = ObjectiveManager.Instance.objectiveConfigs.Find(o => o.itemType == type);

        if (cfg != null && cfg.requiredCount > 0 && rows.ContainsKey(type))
        {
            rows[type].text = $"{cfg.itemType} {cfg.currentCount}/{cfg.requiredCount}";
        }
    }
}
