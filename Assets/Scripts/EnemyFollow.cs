using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;  // Referencia al transform del jugador
    public float moveSpeed = 5f;  // Velocidad de movimiento del enemigo

    private bool isPlayerInRange = false;
    public MeshRenderer renderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            FollowPlayer();
            renderer.material.color = Color.red;
        }
        else renderer.material.color = Color.green; 
    }

    private void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
