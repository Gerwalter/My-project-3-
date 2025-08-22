using UnityEngine;

public class DamageTest : MonoBehaviour
{
    public Health target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (target != null)
            {
                target.OnTakeDamage(10);
            }
        }
    }
}
