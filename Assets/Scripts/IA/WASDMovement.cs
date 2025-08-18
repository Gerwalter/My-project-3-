using UnityEngine;

public class WASDMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical"); 

        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
