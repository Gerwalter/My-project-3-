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
    public PlayerFollower follower;    
    public void Accept()
    {
        follower.Follow = true;
        panel.SetActive(false);
    }

    public void Deny()
    {
        panel.SetActive(false);
    }
}
