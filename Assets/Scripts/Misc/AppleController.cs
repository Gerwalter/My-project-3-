using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour
{

    #region Singleton
    public static AppleController Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("<color=blue>Post Processes</color>")]
    [SerializeField] private Material apple;
    [SerializeField] private string _opacity = "_ASA";
    [SerializeField] private string _mainTexture = "_MainTex";
    [SerializeField] private Texture _Texture;

    // Nueva referencia a la textura "_MainTex"


    public Material VignettePostProcess
    {
        get { return apple; }
        set { apple = value; }
    }

    public string VignetteAmountName
    {
        get { return _opacity; }
    }

    public string MainTextureName
    {
        get { return _mainTexture; }
        
    }

    private void Start()
    {
        if (_mainTexture != null)
        {
            apple.SetTexture("_MainTex", _Texture); // Asigna la textura en Start
        }
    }
}
