using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class ElementalType : MonoBehaviour
{
    public string[] element;
    
    private void Update()
    {
        ElementalCast();
    }
    public void ElementalCast()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            int randomIndex = Random.Range(0, element.Length);
            print(element[randomIndex]);
        }
    }
}
