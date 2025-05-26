using Unity.VisualScripting;
using UnityEngine;

public class Lock : MonoBehaviour
{

    public GameObject menu;
    public MenuCameraLocker Locker;

    private void Start()
    {
        menu.SetActive(false);
        CanvasReferencesManager.Instance.Handle = this;
    }

    public void OnDie()
    {
        menu.SetActive(true);


    }

    private void Update()
    {

    }
}
