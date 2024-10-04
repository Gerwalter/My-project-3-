using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    #region Singleton
    public static FullScreenManager Instance;

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
    [SerializeField] private Material _vignettePP;
    [SerializeField] private string _vigAmountName = "_ScreenBorderAmount";

    public Material VignettePostProcess
    {
        get { return _vignettePP; }
        set { _vignettePP = value; }
    }

    public string VignetteAmountName
    {
        get { return _vigAmountName; }
    }
}
