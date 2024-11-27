using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioClip Theme;
    // Start is called before the first frame update
    void Start()
    {
        SFXManager.instance.PlaySFXClip(Theme, transform, 2f);
    }
}
