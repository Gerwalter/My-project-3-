using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasReferencesManager : MonoBehaviour
{
    #region Singleton
    public static CanvasReferencesManager Instance;

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


    [SerializeField] private Image healthbar;

    public Image Healthbar
    {
        get { return healthbar; }
        set { healthbar = value; }
    }

    [SerializeField] private Image hookTimer;
    public Image HookTimer
    {
        get { return hookTimer; }
        set { hookTimer = value; }
    }

    [SerializeField] private Image crossHair;
    public Image CrossHair
    {
        get { return crossHair; }
        set { crossHair = value; }
    }

    [SerializeField] private TextMeshProUGUI goldText;

    public TextMeshProUGUI GoldText
    {
        get { return goldText; }
        set { goldText = value; }
    }
    [SerializeField] private Lock handle;

    public Lock Handle
    {
        get { return handle; }
        set { handle = value; }
    }
}


