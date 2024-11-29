using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct UltimateAbilities
{
    public string Name;                  // Nombre del ataque
    public float Cost;               // Tiempo de reutilización del ataque
    public GameObject Ability;      // Prefab del ataque para instanciarlo

    public UltimateAbilities(string name, float cost, GameObject ability)
    {
        Name = name;
        Cost = cost;
        Ability = ability;
    }
}
