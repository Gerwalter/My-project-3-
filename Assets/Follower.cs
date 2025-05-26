using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : ButtonBehaviour
{
    public GameObject panel;
    public bool interacting;

    private void Update()
    {
        interacting = panel.activeSelf;
    }
    public override void OnInteract()
    {
        panel.SetActive(true);
    }


    public void Deny()
    {
        panel.SetActive(false);
    }
}
