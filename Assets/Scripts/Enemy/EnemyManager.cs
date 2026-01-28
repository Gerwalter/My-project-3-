using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AttackSlotData
{
    public AttackSlotType type;
    public int maxSlots = 1;
    [HideInInspector] public int currentUsed = 0;
}
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] public float Enemycount;

    [Header("Attack Slot System")]
    [SerializeField] private List<AttackSlotData> attackSlots = new List<AttackSlotData>();
    [Header("Attack Slots")]
    [SerializeField] private int maxAttackers = 1; // Fácil por defecto
    private int currentAttackers = 0;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private AttackSlotData GetSlotData(AttackSlotType type)
    {
        return attackSlots.Find(s => s.type == type);
    }
    private void Start()
    {
        // Buscar todos los enemigos que existan en la escena al inicio
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in foundEnemies)
        {
            RegisterEnemy(e);
        }
    }

    public bool CanRequestSlot(AttackSlotType type)
    {
        var slot = GetSlotData(type);
        if (slot == null) return false;

        return slot.currentUsed < slot.maxSlots;
    }
    public bool RequestSlot(AttackSlotType type)
    {
        var slot = GetSlotData(type);
        if (slot == null) return false;

        if (slot.currentUsed >= slot.maxSlots)
            return false;

        slot.currentUsed++;
        Debug.Log($"[EnemyManager] Slot {type} OCUPADO → {slot.currentUsed}/{slot.maxSlots}");
        return true;
    }
    public void ReleaseSlot(AttackSlotType type)
    {
        var slot = GetSlotData(type);
        if (slot == null) return;

        slot.currentUsed = Mathf.Max(0, slot.currentUsed - 1);
        Debug.Log($"[EnemyManager] Slot {type} LIBERADO → {slot.currentUsed}/{slot.maxSlots}");
    }
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    public bool CanRequestAttack()
    {
        return currentAttackers < maxAttackers;
    }

    public bool RequestAttackSlot()
    {
        if (currentAttackers >= maxAttackers)
            return false;

        currentAttackers++;
        Debug.Log($"[EnemyManager] Slot OCUPADO -> {currentAttackers}/{maxAttackers}");
        return true;
    }

    public void ReleaseAttackSlot()
    {
        currentAttackers = Mathf.Max(0, currentAttackers - 1);
        Debug.Log($"[EnemyManager] Slot LIBERADO -> {currentAttackers}/{maxAttackers}");
    }
    public void SetMaxAttackers(int value)
    {
        maxAttackers = Mathf.Max(1, value);
        Debug.Log($"[EnemyManager] Max atacantes simultáneos = {maxAttackers}");
    }
}
