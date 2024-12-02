using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Canvas Canvas;
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);


        LoadLevelByName("Casino 1");
    }

    public void LoadLevelByName(string levelName)
    {
        // Cargar una escena específica por nombre
        SceneManager.LoadScene(levelName);
        GameManager.Instance.Enemies.Clear();
    }
}
