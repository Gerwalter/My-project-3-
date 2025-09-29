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
        // limpiar y (re)crear
        foreach (Transform t in contentParent) Destroy(t.gameObject);
        rows.Clear();

        if (ObjectiveManager.Instance == null) return;
        foreach (var obj in ObjectiveManager.Instance.objectives)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);
            TextMeshProUGUI t = row.GetComponentInChildren<TextMeshProUGUI>();
            if (t != null)
            {
                t.text = $"{obj.itemType} {obj.currentCount}/{obj.requiredCount}";
                rows[obj.itemType] = t;
            }
        }
    }

    // Método público si quieres actualizar solo un tipo
    public void UpdateRow(ItemType type)
    {
        if (ObjectiveManager.Instance == null) return;
        var obj = ObjectiveManager.Instance.objectives.Find(o => o.itemType == type);
        if (obj != null && rows.ContainsKey(type))
            rows[type].text = $"{obj.itemType} {obj.currentCount}/{obj.requiredCount}";
    }
}
