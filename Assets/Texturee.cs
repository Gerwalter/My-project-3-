using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texturee : MonoBehaviour
{
    // Referencias a los materiales que usarán los shaders
    public Material material1; // Asigna el primer material
    public Material material2; // Asigna el segundo material
    public Material material3; // Asigna el segundo material

    // Diccionarios para manejar el estado de cada keyword por material
    private Dictionary<string, bool> material1KeywordStates = new Dictionary<string, bool>();
    private Dictionary<string, bool> material2KeywordStates = new Dictionary<string, bool>();
    private Dictionary<string, bool> material3KeywordStates = new Dictionary<string, bool>();

    // Método para alternar una keyword en el primer material
    public void ToggleKeywordMaterial1(string keyword)
    {
        // Si la keyword aún no tiene un estado asignado, la inicializa en true
        if (!material1KeywordStates.ContainsKey(keyword))
        {
            material1KeywordStates[keyword] = true;
        }

        // Alterna el estado de la keyword en el primer material
        if (material1KeywordStates[keyword])
        {
            material1.DisableKeyword(keyword);
        }
        else
        {
            material1.EnableKeyword(keyword);
        }

        // Actualiza el estado de la keyword en el diccionario para el primer material
        material1KeywordStates[keyword] = !material1KeywordStates[keyword];
    }

    // Método para alternar una keyword en el segundo material
    public void ToggleKeywordMaterial2(string keyword)
    {
        // Si la keyword aún no tiene un estado asignado, la inicializa en true
        if (!material2KeywordStates.ContainsKey(keyword))
        {
            material2KeywordStates[keyword] = true;
        }

        // Alterna el estado de la keyword en el segundo material
        if (material2KeywordStates[keyword])
        {
            material2.DisableKeyword(keyword);
        }
        else
        {
            material2.EnableKeyword(keyword);
        }

        // Actualiza el estado de la keyword en el diccionario para el segundo material
        material2KeywordStates[keyword] = !material2KeywordStates[keyword];
    }
    public void ToggleKeywordMaterial3(string keyword)
    {
        // Si la keyword aún no tiene un estado asignado, la inicializa en true
        if (!material3KeywordStates.ContainsKey(keyword))
        {
            material3KeywordStates[keyword] = true;
        }

        // Alterna el estado de la keyword en el segundo material
        if (material3KeywordStates[keyword])
        {
            material3.EnableKeyword(keyword); 
        }
        else
        {
            material3.DisableKeyword(keyword);
        }

        // Actualiza el estado de la keyword en el diccionario para el segundo material
        material3KeywordStates[keyword] = !material3KeywordStates[keyword];
    }
}
