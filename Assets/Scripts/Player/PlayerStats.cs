using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int gold;
    private void Awake()
    {
        GameManager.Instance.Stats = this;
    }
    public void AddLoot(LootData loot)
    {
        gold += loot.gold;
    }
}
