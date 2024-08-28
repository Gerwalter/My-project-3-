using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipsTracker : MonoBehaviour
{
    public GameObject player;
    public GameObject player2;
    public GameObject player3;
    public PlayerReactivator reactivator;
 // Referencia al segundo script

    public void TrackerHips()
    {
        reactivator.ReactivatePlayerAfterDelay();
        player.SetActive(!player.activeSelf);
        player2.SetActive(!player2.activeSelf);
        player3.SetActive(!player3.activeSelf);
    }
}
//(-1.09, 0.88, 2.70)