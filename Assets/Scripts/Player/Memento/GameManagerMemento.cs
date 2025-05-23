using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMemento : MonoBehaviour
{
    Rewind[] _rewinds;
    Coroutine _CoroutineSaved;
    void Awake()
    {
        _rewinds = FindObjectsOfType<Rewind>();    
    }

    
    public void Saver()
    {
        print("AAAAAA");
        _CoroutineSaved = StartCoroutine(CoroutineSave());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_CoroutineSaved != null)
                StopCoroutine(_CoroutineSaved);

            _CoroutineSaved = StartCoroutine(CoroutineLoad());
        }
    }

    IEnumerator CoroutineSave()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);
        while (true) 
        {
            foreach (var item in _rewinds)
                item.Save();

            yield return WaitForSeconds;
        }
    }

    IEnumerator CoroutineLoad()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);

        bool finishLoad = false;
        while (finishLoad == false)
        {
            finishLoad = true;
            foreach (var item in _rewinds)
            {
                if (item.mementoState.IsRemember())
                    finishLoad = false;

                item.Load();
            }

            yield return WaitForSeconds;
        }

    }
}
