using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float life;
    [SerializeField] protected float speed;

    protected void Move(Vector3 target)
    {
        transform.position += (target - transform.position).normalized * Time.deltaTime * speed;
    }

    public virtual void Move()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    //protected void Move(Vector3 horizontal, Vector3 vertical)
    //{
    //    transform.position += (vertical + horizontal).normalized * Time.deltaTime * speed;
    //}
}
