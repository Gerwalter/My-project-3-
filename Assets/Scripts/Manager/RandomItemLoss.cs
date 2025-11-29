using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomItemLoss : MonoBehaviour
{    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    public string nextSceneName = "MainLevel";
    private bool alreadyExecuted = false;
    private void Start()
    {
        EventManager.Subscribe("Death", PlayerDeath);
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        EventManager.Unsubscribe("Death", PlayerDeath);

        EventManager.Subscribe("Death", PlayerDeath);
        alreadyExecuted = false ;
    }
    private void PlayerDeath(params object[] args)
    {
        ApplyRandomLoss();
    }
    public void ApplyRandomLoss()
    {
        if (alreadyExecuted)
        {
            Debug.LogWarning("ApplyRandomLoss ya fue ejecutado. Ignorando llamada duplicada.");
            return;
        }
        alreadyExecuted = true;
        foreach (var cfg in ObjectiveManager.Instance.objectiveConfigs)
        {
            int current = cfg.currentCount;

            // Si no tiene items, no le quitamos nada
            if (current <= 0)
            {
                Debug.Log($"No hay {cfg.itemType} para restar.");
                continue;
            }

            // Cantidad a restar entre 0 y el total actual
            int toRemove = Random.Range(0, current + 1);

            cfg.currentCount -= toRemove;
            if (cfg.currentCount < 0) cfg.currentCount = 0;

            // Quitar puntos usando tu sistema actual
            float puntosAQuitar = toRemove * ObjectiveManager.Instance.valuePerItem;
            EventManager.Trigger("DecreasePoint", puntosAQuitar);

            Debug.Log($"Perdiste {toRemove}x {cfg.itemType}. Total ahora: {cfg.currentCount}. Puntos -{puntosAQuitar}");
        }
        LoadingScreen.nextScene = nextSceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}

