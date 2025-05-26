using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUnlockManager : MonoBehaviour
{
    public static ComboUnlockManager Instance;

    private HashSet<string> unlockedCombos = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public bool IsUnlocked(string comboID)
    {
        return unlockedCombos.Contains(comboID);
    }

    public void UnlockCombo(string comboID)
    {
        if (!string.IsNullOrEmpty(comboID))
            unlockedCombos.Add(comboID);
    }
}
